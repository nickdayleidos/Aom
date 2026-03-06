using Microsoft.EntityFrameworkCore;
using MyApplication.Common.Time;
using MyApplication.Components.Model.AOM.Employee;

namespace MyApplication.Components.Service
{
    public sealed partial class OperaRepository
    {
        // Sub-type IDs excluded from the "impacting" classification (OutageType == 1)
        private static readonly int[] ImpactingExcludedSubTypeIds = new[] { 1, 4, 7, 9, 10, 12, 13, 14, 47, 53, 54 };

        public async Task SaveOperaRequestAsync(OperaRequest request, CancellationToken ct = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            ApplyTimeframeRulesAndComputeImpact(request);

            await using var db = await _factory.CreateDbContextAsync(ct);
            db.OperaRequests.Add(request);
            await db.SaveChangesAsync(ct);
        }

        public async Task UpdateRequestAsync(OperaRequest request, CancellationToken ct = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            ApplyTimeframeRulesAndComputeImpact(request);

            await using var db = await _factory.CreateDbContextAsync(ct);
            db.OperaRequests.Update(request);
            await db.SaveChangesAsync(ct);
        }

        public async Task<int> CreateManyAsync(IEnumerable<OperaRequest> requests, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            foreach (var r in requests)
                ApplyTimeframeRulesAndComputeImpact(r);

            await db.OperaRequests.AddRangeAsync(requests, ct);
            return await db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(OperaRequest req, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);

            var existing = await db.OperaRequests.FirstOrDefaultAsync(x => x.RequestId == req.RequestId, ct);
            if (existing != null)
            {
                existing.StartTime = req.StartTime;
                existing.EndTime = req.EndTime;
                existing.ActivityTypeId = req.ActivityTypeId;
                existing.ActivitySubTypeId = req.ActivitySubTypeId;
                existing.SubmitterComments = req.SubmitterComments;
                existing.TimeframeId = req.TimeframeId;
                existing.IsImpacting = req.IsImpacting;
                existing.OperaStatusId = req.OperaStatusId;

                if (existing.OperaStatusId == 1 || existing.OperaStatusId == 6)
                {
                    existing.ApproveTime = null;
                    existing.ApproveBy = null;
                    existing.RejectedTime = null;
                    existing.RejectedBy = null;
                    existing.CancelledTime = null;
                    existing.CancelledBy = null;
                }

                existing.LastUpdatedTime = Et.Now;
                existing.LastUpdatedBy = req.LastUpdatedBy;

                await db.SaveChangesAsync(ct);
            }
        }

        public async Task SetStatusAsync(int requestId, int statusId, string actor, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var r = await db.OperaRequests.FirstOrDefaultAsync(x => x.RequestId == requestId, ct);
            if (r == null) return;

            r.OperaStatusId = statusId;
            var nowEt = Et.Now;

            if (statusId == 1 || statusId == 6)
            {
                r.ApproveTime = null;
                r.ApproveBy = null;
                r.RejectedTime = null;
                r.RejectedBy = null;
                r.CancelledTime = null;
                r.CancelledBy = null;
            }

            if (statusId == 2) { r.ApproveTime = nowEt; r.ApproveBy = actor; }
            else if (statusId == 5) { r.RejectedTime = nowEt; r.RejectedBy = actor; }
            else if (statusId == 4) { r.CancelledTime = nowEt; r.CancelledBy = actor; }

            r.LastUpdatedTime = nowEt;
            r.LastUpdatedBy = actor;

            await db.SaveChangesAsync(ct);
        }

        // Business rules: timeframe normalization + impact classification
        private void ApplyTimeframeRulesAndComputeImpact(OperaRequest req)
        {
            if (req == null) return;

            switch (req.TimeframeId)
            {
                case 2:
                    req.EndTime = req.StartTime.Date.AddHours(23).AddMinutes(59);
                    break;
                case 3:
                    if (req.EndTime < req.StartTime)
                        throw new ArgumentException("End date must be >= start date for Multiple Full Day timeframe.");
                    break;
                case 4:
                    var candidate = new DateTime(req.StartTime.Year, req.StartTime.Month, req.StartTime.Day,
                                                 req.EndTime.Hour, req.EndTime.Minute, req.EndTime.Second);
                    if (candidate <= req.StartTime) candidate = candidate.AddDays(1);
                    req.EndTime = candidate;
                    break;
            }

            var midnightEt = req.StartTime.Date;
            req.IsImpacting = req.ActivityTypeId == 1
                              && !ImpactingExcludedSubTypeIds.Contains(req.ActivitySubTypeId)
                              && req.SubmitTime > midnightEt;
        }
    }
}
