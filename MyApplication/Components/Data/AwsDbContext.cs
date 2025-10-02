using Microsoft.EntityFrameworkCore;
using MyApplication.Components.Model.AWS;

using System;
using System.Collections.Generic;

namespace MyApplication;

public partial class AwsDbContext : DbContext
{
    public AwsDbContext()
    {
    }

    public AwsDbContext(DbContextOptions<AwsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AgentProfile> AgentProfiles { get; set; }

    public virtual DbSet<AgentRoutingProfileView> AgentRoutingProfileViews { get; set; }

    public virtual DbSet<AsadailyQuickReference> AsadailyQuickReferences { get; set; }

    public virtual DbSet<Asaquick> Asaquicks { get; set; }

    public virtual DbSet<AwsAgent> AwsAgents { get; set; }

    public virtual DbSet<AwsAhttarget> AwsAhttargets { get; set; }

    public virtual DbSet<AwsCallDataTemp> AwsCallDataTemps { get; set; }

    public virtual DbSet<AwsCallDatum> AwsCallData { get; set; }

    public virtual DbSet<AwsCallQueue> AwsCallQueues { get; set; }

    public virtual DbSet<AwsCallType> AwsCallTypes { get; set; }

    public virtual DbSet<AwsChannel> AwsChannels { get; set; }

    public virtual DbSet<AwsDetailedAgentDataTemp> AwsDetailedAgentDataTemps { get; set; }

    public virtual DbSet<AwsDetailedAgentDatum> AwsDetailedAgentData { get; set; }

    public virtual DbSet<AwsInitiationMethod> AwsInitiationMethods { get; set; }

    public virtual DbSet<AwsIntervalDatum> AwsIntervalData { get; set; }

    public virtual DbSet<AwsQueuesToProfile> AwsQueuesToProfiles { get; set; }

    public virtual DbSet<AwsRoutingProfile> AwsRoutingProfiles { get; set; }

    public virtual DbSet<AwsRoutingProfileIndex> AwsRoutingProfileIndices { get; set; }

    public virtual DbSet<AwsagentStatusDaily> AwsagentStatusDailies { get; set; }

    public virtual DbSet<CallsByAgent> CallsByAgents { get; set; }

    public virtual DbSet<CallsByAgentByDay> CallsByAgentByDays { get; set; }

    public virtual DbSet<CallsByAgentByQueueByDay> CallsByAgentByQueueByDays { get; set; }

    public virtual DbSet<ChatTemp> ChatTemps { get; set; }

    public virtual DbSet<HsplitCallbackdatum> HsplitCallbackdata { get; set; }

    public virtual DbSet<HsplitTest> HsplitTests { get; set; }

    public virtual DbSet<HsplitdataOld> HsplitdataOlds { get; set; }

    public virtual DbSet<Hsplitdatum> Hsplitdata { get; set; }

    public virtual DbSet<RocCallbackToolUpdate> RocCallbackToolUpdates { get; set; }

    public virtual DbSet<TblAwsProfile> TblAwsProfiles { get; set; }

    public virtual DbSet<TblSplitDef> TblSplitDefs { get; set; }

    public virtual DbSet<TblSplitDefOld> TblSplitDefOlds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:AWS");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgentProfile>(entity =>
        {
            entity.HasKey(e => e.Employeeid).HasName("PK__AgentPro__C135F5E9273476F2");

            entity.Property(e => e.Employeeid)
                .ValueGeneratedNever()
                .HasColumnName("employeeid");
            entity.Property(e => e.AdditionalSkill)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Previousprofileid).HasColumnName("previousprofileid");
            entity.Property(e => e.Weekdayprofileid).HasColumnName("weekdayprofileid");
            entity.Property(e => e.Weekendprofileid).HasColumnName("weekendprofileid");
        });

        modelBuilder.Entity<AgentRoutingProfileView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("agentRoutingProfileView");

            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.WeekdayProfile)
                .HasMaxLength(255)
                .HasColumnName("weekdayProfile");
            entity.Property(e => e.WeekendProfile)
                .HasMaxLength(255)
                .HasColumnName("weekendProfile");
        });

        modelBuilder.Entity<AsadailyQuickReference>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ASADailyQuickReference");

            entity.Property(e => e.AsaMtd).HasColumnName("AsaMTD");
            entity.Property(e => e.Asadaily).HasColumnName("ASADaily");
            entity.Property(e => e.Coi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COI");
        });

        modelBuilder.Entity<Asaquick>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ASAquick");

            entity.Property(e => e.Anstime).HasColumnName("anstime");
            entity.Property(e => e.AnsweredCallsMtd).HasColumnName("AnsweredCallsMTD");
            entity.Property(e => e.AsaTableId).HasColumnName("AsaTableID");
            entity.Property(e => e.Asadaily)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ASADaily");
            entity.Property(e => e.CallsAbandonedMtd).HasColumnName("CallsAbandonedMTD");
            entity.Property(e => e.CallsOfferedMtd).HasColumnName("CallsOfferedMTD");
            entity.Property(e => e.Coi)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("COI");
            entity.Property(e => e.Month).HasMaxLength(30);
            entity.Property(e => e.Mtdasa)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("MTDasa");
        });

        modelBuilder.Entity<AwsAgent>(entity =>
        {
            entity.Property(e => e.AwsGuid).HasMaxLength(50);
            entity.Property(e => e.AwsUserName).HasMaxLength(50);
        });

        modelBuilder.Entity<AwsAhttarget>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("awsAHTTargets");

            entity.Property(e => e.AcdCalls).HasColumnName("acdCalls");
            entity.Property(e => e.AhtTargetId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AHT_Target_ID");
            entity.Property(e => e.AhtTargetMonth).HasColumnName("AHT_Target_Month");
            entity.Property(e => e.EntrAhtTarget)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ENTR_AHT_Target");
            entity.Property(e => e.QueueName)
                .HasMaxLength(50)
                .HasColumnName("queue_name");
            entity.Property(e => e.Tht).HasColumnName("THT");
        });

        modelBuilder.Entity<AwsCallDataTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("awsCallData_temp");

            entity.Property(e => e.AgentAftercontactworkduration).HasColumnName("agent_aftercontactworkduration");
            entity.Property(e => e.AgentAftercontactworkendtimestamp)
                .HasPrecision(0)
                .HasColumnName("agent_aftercontactworkendtimestamp");
            entity.Property(e => e.AgentAftercontactworkstarttimestamp)
                .HasPrecision(0)
                .HasColumnName("agent_aftercontactworkstarttimestamp");
            entity.Property(e => e.AgentAgentinteractionduration).HasColumnName("agent_agentinteractionduration");
            entity.Property(e => e.AgentConnectedtoagenttimestamp)
                .HasPrecision(0)
                .HasColumnName("agent_connectedtoagenttimestamp");
            entity.Property(e => e.AgentCustomerholdduration).HasColumnName("agent_customerholdduration");
            entity.Property(e => e.AgentLongestholdduration).HasColumnName("agent_longestholdduration");
            entity.Property(e => e.AgentNumberofholds).HasColumnName("agent_numberofholds");
            entity.Property(e => e.AgentRoutingprofileName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_routingprofile_name");
            entity.Property(e => e.AgentUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_username");
            entity.Property(e => e.Agentconnectionattempts).HasColumnName("agentconnectionattempts");
            entity.Property(e => e.AttributesCallbacknumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("attributes_callbacknumber");
            entity.Property(e => e.AttributesCallcategory)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("attributes_callcategory");
            entity.Property(e => e.AttributesCalltype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("attributes_calltype");
            entity.Property(e => e.AttributesUsername)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("attributes_username");
            entity.Property(e => e.AwsGuid)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Channel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("channel");
            entity.Property(e => e.Connectedtosystemtimestamp)
                .HasPrecision(0)
                .HasColumnName("connectedtosystemtimestamp");
            entity.Property(e => e.Contactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contactid");
            entity.Property(e => e.CustomerendpointAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("customerendpoint_address");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.Disconnecttimestamp)
                .HasPrecision(0)
                .HasColumnName("disconnecttimestamp");
            entity.Property(e => e.Initialcontactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initialcontactid");
            entity.Property(e => e.Initiationmethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initiationmethod");
            entity.Property(e => e.Initiationtimestamp)
                .HasPrecision(0)
                .HasColumnName("initiationtimestamp");
            entity.Property(e => e.Lastupdatetimestamp)
                .HasPrecision(0)
                .HasColumnName("lastupdatetimestamp");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.Nextcontactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nextcontactid");
            entity.Property(e => e.Previouscontactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("previouscontactid");
            entity.Property(e => e.QueueDequeuetimestamp)
                .HasPrecision(0)
                .HasColumnName("queue_dequeuetimestamp");
            entity.Property(e => e.QueueDuration).HasColumnName("queue_duration");
            entity.Property(e => e.QueueEnqueuetimestamp)
                .HasPrecision(0)
                .HasColumnName("queue_enqueuetimestamp");
            entity.Property(e => e.QueueName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("queue_name");
            entity.Property(e => e.Recordings0Starttimestamp)
                .HasPrecision(0)
                .HasColumnName("recordings_0_starttimestamp");
            entity.Property(e => e.Recordings0Stoptimestamp)
                .HasPrecision(0)
                .HasColumnName("recordings_0_stoptimestamp");
            entity.Property(e => e.SystemendpointAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("systemendpoint_address");
            entity.Property(e => e.Transfercompletedtimestamp)
                .HasPrecision(0)
                .HasColumnName("transfercompletedtimestamp");
            entity.Property(e => e.Transferredtoendpoint)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("transferredtoendpoint");
            entity.Property(e => e.TransferredtoendpointAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("transferredtoendpoint_address");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<AwsCallDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("awsCallData");

            entity.HasIndex(e => e.AwsGuid, "IDX_awsCallData_AwsGuid");

            entity.HasIndex(e => new { e.Year, e.Month, e.Day }, "IDX_awsCallData_YearMonthDay");

            entity.HasIndex(e => new { e.Initiationmethod, e.QueueName, e.AgentUsername, e.Initiationtimestamp }, "IDX_awsCallData_commonFilters");

            entity.HasIndex(e => e.Contactid, "IDX_awsCallData_contactid");

            entity.HasIndex(e => e.QueueName, "IDX_awsCallData_queue_name");

            entity.Property(e => e.AgentAftercontactworkduration).HasColumnName("agent_aftercontactworkduration");
            entity.Property(e => e.AgentAftercontactworkendtimestamp)
                .HasPrecision(0)
                .HasColumnName("agent_aftercontactworkendtimestamp");
            entity.Property(e => e.AgentAftercontactworkstarttimestamp)
                .HasPrecision(0)
                .HasColumnName("agent_aftercontactworkstarttimestamp");
            entity.Property(e => e.AgentAgentinteractionduration).HasColumnName("agent_agentinteractionduration");
            entity.Property(e => e.AgentConnectedtoagenttimestamp)
                .HasPrecision(0)
                .HasColumnName("agent_connectedtoagenttimestamp");
            entity.Property(e => e.AgentCustomerholdduration).HasColumnName("agent_customerholdduration");
            entity.Property(e => e.AgentLongestholdduration).HasColumnName("agent_longestholdduration");
            entity.Property(e => e.AgentNumberofholds).HasColumnName("agent_numberofholds");
            entity.Property(e => e.AgentRoutingprofileName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_routingprofile_name");
            entity.Property(e => e.AgentUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_username");
            entity.Property(e => e.Agentconnectionattempts).HasColumnName("agentconnectionattempts");
            entity.Property(e => e.AttributesCallbacknumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("attributes_callbacknumber");
            entity.Property(e => e.AttributesCallcategory)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("attributes_callcategory");
            entity.Property(e => e.AttributesCalltype)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("attributes_calltype");
            entity.Property(e => e.AttributesUsername)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("attributes_username");
            entity.Property(e => e.AwsGuid)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Channel)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("channel");
            entity.Property(e => e.Connectedtosystemtimestamp)
                .HasPrecision(0)
                .HasColumnName("connectedtosystemtimestamp");
            entity.Property(e => e.Contactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contactid");
            entity.Property(e => e.CustomerendpointAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("customerendpoint_address");
            entity.Property(e => e.Day).HasColumnName("day");
            entity.Property(e => e.Disconnecttimestamp)
                .HasPrecision(0)
                .HasColumnName("disconnecttimestamp");
            entity.Property(e => e.Initialcontactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initialcontactid");
            entity.Property(e => e.Initiationmethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initiationmethod");
            entity.Property(e => e.Initiationtimestamp)
                .HasPrecision(0)
                .HasColumnName("initiationtimestamp");
            entity.Property(e => e.Lastupdatetimestamp)
                .HasPrecision(0)
                .HasColumnName("lastupdatetimestamp");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.Nextcontactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nextcontactid");
            entity.Property(e => e.Previouscontactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("previouscontactid");
            entity.Property(e => e.QueueDequeuetimestamp)
                .HasPrecision(0)
                .HasColumnName("queue_dequeuetimestamp");
            entity.Property(e => e.QueueDuration).HasColumnName("queue_duration");
            entity.Property(e => e.QueueEnqueuetimestamp)
                .HasPrecision(0)
                .HasColumnName("queue_enqueuetimestamp");
            entity.Property(e => e.QueueName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("queue_name");
            entity.Property(e => e.Recordings0Starttimestamp)
                .HasPrecision(0)
                .HasColumnName("recordings_0_starttimestamp");
            entity.Property(e => e.Recordings0Stoptimestamp)
                .HasPrecision(0)
                .HasColumnName("recordings_0_stoptimestamp");
            entity.Property(e => e.SystemendpointAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("systemendpoint_address");
            entity.Property(e => e.Transfercompletedtimestamp)
                .HasPrecision(0)
                .HasColumnName("transfercompletedtimestamp");
            entity.Property(e => e.Transferredtoendpoint)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("transferredtoendpoint");
            entity.Property(e => e.TransferredtoendpointAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("transferredtoendpoint_address");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<AwsCallQueue>(entity =>
        {
            entity.HasKey(e => e.QueueId).HasName("PK__awsCallQ__7E27159E2E3FD086");

            entity.ToTable("awsCallQueues");

            entity.Property(e => e.QueueId).HasColumnName("queueId");
            entity.Property(e => e.Bucket)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("bucket");
            entity.Property(e => e.CallGroupSpecial)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("callGroupSpecial");
            entity.Property(e => e.Coi)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COI");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Enabled).HasColumnName("enabled");
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
            entity.Property(e => e.Queue)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("queue");
        });

        modelBuilder.Entity<AwsCallType>(entity =>
        {
            entity.ToTable("AwsCallType");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CallType).HasMaxLength(20);
        });

        modelBuilder.Entity<AwsChannel>(entity =>
        {
            entity.ToTable("AwsChannel");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Channel)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        modelBuilder.Entity<AwsDetailedAgentDataTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("awsDetailedAgentDataTemp");

            entity.Property(e => e.AwsGuid).HasMaxLength(50);
            entity.Property(e => e.AwsId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("awsId");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("changeType");
            entity.Property(e => e.CurrentAgentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("currentAgentStatus");
            entity.Property(e => e.CurrentRoutingProfile)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("currentRoutingProfile");
            entity.Property(e => e.CurrentStatusType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("currentStatusType");
            entity.Property(e => e.EventTimeEt)
                .HasColumnType("datetime")
                .HasColumnName("eventTimeET");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("eventType");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.PrevAgentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("prevAgentStatus");
            entity.Property(e => e.PrevRoutingProfile)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("prevRoutingProfile");
            entity.Property(e => e.PrevStatusType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("prevStatusType");
        });

        modelBuilder.Entity<AwsDetailedAgentDatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("awsDetailedAgentData");

            entity.HasIndex(e => e.EventId, "UQ_eventid").IsUnique();

            entity.Property(e => e.AwsGuid).HasMaxLength(50);
            entity.Property(e => e.AwsId)
                .HasMaxLength(50)
                .HasColumnName("awsId");
            entity.Property(e => e.ChangeType)
                .HasMaxLength(50)
                .HasColumnName("changeType");
            entity.Property(e => e.CurrentAgentStatus)
                .HasMaxLength(50)
                .HasColumnName("currentAgentStatus");
            entity.Property(e => e.CurrentRoutingProfile)
                .HasMaxLength(50)
                .HasColumnName("currentRoutingProfile");
            entity.Property(e => e.CurrentStatusType)
                .HasMaxLength(50)
                .HasColumnName("currentStatusType");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.EventId).HasColumnName("eventId");
            entity.Property(e => e.EventTimeEt)
                .HasColumnType("datetime")
                .HasColumnName("eventTimeET");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .HasColumnName("eventType");
            entity.Property(e => e.PrevAgentStatus)
                .HasMaxLength(50)
                .HasColumnName("prevAgentStatus");
            entity.Property(e => e.PrevRoutingProfile)
                .HasMaxLength(50)
                .HasColumnName("prevRoutingProfile");
            entity.Property(e => e.PrevStatusType)
                .HasMaxLength(50)
                .HasColumnName("prevStatusType");
        });

        modelBuilder.Entity<AwsInitiationMethod>(entity =>
        {
            entity.Property(e => e.InitiationMethod)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        modelBuilder.Entity<AwsIntervalDatum>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.AbnCallsAsa).HasColumnName("abnCallsASA");
            entity.Property(e => e.AbnTimeAsa).HasColumnName("abnTimeASA");
            entity.Property(e => e.Abncalls).HasColumnName("abncalls");
            entity.Property(e => e.Abntime).HasColumnName("abntime");
            entity.Property(e => e.Acdcalls).HasColumnName("acdcalls");
            entity.Property(e => e.Acdtime).HasColumnName("acdtime");
            entity.Property(e => e.Acwtime).HasColumnName("acwtime");
            entity.Property(e => e.Anstime).HasColumnName("anstime");
            entity.Property(e => e.Callbacks).HasColumnName("callbacks");
            entity.Property(e => e.CallsOffered).HasColumnName("callsOffered");
            entity.Property(e => e.DistinctAgent).HasColumnName("distinctAgent");
            entity.Property(e => e.EDate).HasColumnName("eDate");
            entity.Property(e => e.Et)
                .HasPrecision(0)
                .HasColumnName("et");
            entity.Property(e => e.Holdcalls).HasColumnName("holdcalls");
            entity.Property(e => e.Holdtime).HasColumnName("holdtime");
            entity.Property(e => e.InitiationMethod)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("initiationMethod");
            entity.Property(e => e.Interval)
                .HasPrecision(0)
                .HasColumnName("interval");
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
            entity.Property(e => e.QueueId).HasColumnName("queueId");
            entity.Property(e => e.QueueName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("queueName");
            entity.Property(e => e.Voicemails).HasColumnName("voicemails");
        });

        modelBuilder.Entity<AwsQueuesToProfile>(entity =>
        {
            entity.HasKey(e => e.QueueProfileId).HasName("PK_newQueueProfileId");

            entity.ToTable("awsQueuesToProfiles");

            entity.Property(e => e.QueueProfileId).HasColumnName("queueProfileId");
            entity.Property(e => e.QueueId).HasColumnName("queueId");
            entity.Property(e => e.RoutingProfileId).HasColumnName("routingProfileId");
        });

        modelBuilder.Entity<AwsRoutingProfile>(entity =>
        {
            entity.HasKey(e => e.RoutingProfileId).HasName("PK__awsRouti__1C4A60FF5F0EBBC1");

            entity.ToTable("awsRoutingProfiles");

            entity.Property(e => e.RoutingProfileId).HasColumnName("routingProfileId");
            entity.Property(e => e.Coi)
                .HasMaxLength(255)
                .HasColumnName("COI");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Enabled).HasColumnName("enabled");
            entity.Property(e => e.LastUpdated)
                .HasColumnType("datetime")
                .HasColumnName("lastUpdated");
            entity.Property(e => e.ReqNcis).HasColumnName("reqNCIS");
            entity.Property(e => e.ReqNnpi).HasColumnName("reqNNPI");
            entity.Property(e => e.ReqVip).HasColumnName("reqVIP");
            entity.Property(e => e.RoutingProfile)
                .HasMaxLength(255)
                .HasColumnName("routingProfile");
        });

        modelBuilder.Entity<AwsRoutingProfileIndex>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AwsRouti__3214EC07881065E7");

            entity.ToTable("AwsRoutingProfileIndex");

            entity.Property(e => e.RoutingProfileName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AwsagentStatusDaily>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AWSAgentStatusDaily");

            entity.Property(e => e.AdminTasksTime)
                .HasMaxLength(255)
                .HasColumnName("Admin Tasks time");
            entity.Property(e => e.Agent).HasMaxLength(255);
            entity.Property(e => e.AgentAnswerRate).HasColumnName("Agent answer rate");
            entity.Property(e => e.AgentIdleTime).HasColumnName("Agent idle time");
            entity.Property(e => e.AgentOnContactTime).HasColumnName("Agent on contact time");
            entity.Property(e => e.AverageAfterContactWorkTime).HasColumnName("Average after contact work time");
            entity.Property(e => e.AverageAgentInteractionTime).HasColumnName("Average agent interaction time");
            entity.Property(e => e.AverageCustomerHoldTime).HasColumnName("Average customer hold time");
            entity.Property(e => e.AverageHandleTime).HasColumnName("Average handle time");
            entity.Property(e => e.BreakTime).HasColumnName("Break time");
            entity.Property(e => e.CoachingTime).HasColumnName("Coaching time");
            entity.Property(e => e.ContactsHandled).HasColumnName("Contacts handled");
            entity.Property(e => e.ContactsMissed)
                .HasMaxLength(255)
                .HasColumnName("Contacts missed");
            entity.Property(e => e.ContactsTransferredOutByAgent).HasColumnName("Contacts transferred out by agent");
            entity.Property(e => e.EndInterval).HasMaxLength(255);
            entity.Property(e => e.ErrorStatusTime)
                .HasMaxLength(255)
                .HasColumnName("Error status time");
            entity.Property(e => e.LunchTime).HasColumnName("Lunch time");
            entity.Property(e => e.MeetingTime)
                .HasMaxLength(255)
                .HasColumnName("Meeting time");
            entity.Property(e => e.NonproductiveTime).HasColumnName("Nonproductive time");
            entity.Property(e => e.OlCustomerWorkTime).HasColumnName("OL Customer Work time");
            entity.Property(e => e.OnlineTime).HasColumnName("Online time");
            entity.Property(e => e.PeerMentoringTime)
                .HasMaxLength(255)
                .HasColumnName("Peer Mentoring time");
            entity.Property(e => e.RoutingProfile)
                .HasMaxLength(255)
                .HasColumnName("Routing Profile");
            entity.Property(e => e.StartInterval).HasMaxLength(255);
            entity.Property(e => e.SystemTime).HasColumnName("System time");
            entity.Property(e => e.TrainingTime).HasColumnName("Training time");
        });

        modelBuilder.Entity<CallsByAgent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("CallsByAgent");

            entity.Property(e => e.AcdTime).HasColumnName("acdTime");
            entity.Property(e => e.AcwTime).HasColumnName("acwTime");
            entity.Property(e => e.AgentUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_username");
            entity.Property(e => e.EDate).HasColumnName("eDate");
            entity.Property(e => e.Et).HasColumnName("et");
            entity.Property(e => e.HoldTime).HasColumnName("holdTime");
        });

        modelBuilder.Entity<CallsByAgentByDay>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("callsByAgentByDay");

            entity.Property(e => e.AcdCalls).HasColumnName("acdCalls");
            entity.Property(e => e.AcdTime).HasColumnName("acdTime");
            entity.Property(e => e.AcwTime).HasColumnName("acwTime");
            entity.Property(e => e.AgentUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_username");
            entity.Property(e => e.Callbacks).HasColumnName("callbacks");
            entity.Property(e => e.EDate).HasColumnName("eDate");
            entity.Property(e => e.HoldTime).HasColumnName("holdTime");
            entity.Property(e => e.OutboundCalls).HasColumnName("outboundCalls");
            entity.Property(e => e.TotalContacts).HasColumnName("totalContacts");
        });

        modelBuilder.Entity<CallsByAgentByQueueByDay>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("callsByAgentByQueueByDay");

            entity.Property(e => e.AcdCalls).HasColumnName("acdCalls");
            entity.Property(e => e.AcdTime).HasColumnName("acdTime");
            entity.Property(e => e.AcwTime).HasColumnName("acwTime");
            entity.Property(e => e.AgentUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agent_username");
            entity.Property(e => e.Callbacks).HasColumnName("callbacks");
            entity.Property(e => e.EDate).HasColumnName("eDate");
            entity.Property(e => e.HoldTime).HasColumnName("holdTime");
            entity.Property(e => e.OutboundCalls).HasColumnName("outboundCalls");
            entity.Property(e => e.QueueName)
                .HasMaxLength(8000)
                .IsUnicode(false)
                .HasColumnName("queue_name");
            entity.Property(e => e.TotalContacts).HasColumnName("totalContacts");
        });

        modelBuilder.Entity<ChatTemp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("chat_temp");

            entity.Property(e => e.AttributesUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("attributes_username");
            entity.Property(e => e.Contactid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contactid");
            entity.Property(e => e.Initiationtimestamp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initiationtimestamp");
        });

        modelBuilder.Entity<HsplitCallbackdatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("hsplitCallbackdata");

            entity.Property(e => e.Abncalls).HasColumnName("abncalls");
            entity.Property(e => e.Abntime).HasColumnName("abntime");
            entity.Property(e => e.Acdcalls).HasColumnName("acdcalls");
            entity.Property(e => e.Acdtime).HasColumnName("acdtime");
            entity.Property(e => e.Acwtime).HasColumnName("acwtime");
            entity.Property(e => e.Anstime).HasColumnName("anstime");
            entity.Property(e => e.Callbacks).HasColumnName("callbacks");
            entity.Property(e => e.Callsoffered).HasColumnName("callsoffered");
            entity.Property(e => e.CmsEquivalent).HasColumnName("CMS_Equivalent");
            entity.Property(e => e.DistinctAgent).HasColumnName("distinct_agent");
            entity.Property(e => e.EDate).HasColumnName("eDate");
            entity.Property(e => e.Et)
                .HasColumnType("datetime")
                .HasColumnName("et");
            entity.Property(e => e.Holdcalls).HasColumnName("holdcalls");
            entity.Property(e => e.Holdtime).HasColumnName("holdtime");
            entity.Property(e => e.Initiationmethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initiationmethod");
            entity.Property(e => e.Interval)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("interval");
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
            entity.Property(e => e.QueueName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("queue_name");
            entity.Property(e => e.Voicemails).HasColumnName("voicemails");
        });

        modelBuilder.Entity<HsplitTest>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("hsplit_test");

            entity.Property(e => e.Abncalls).HasColumnName("abncalls");
            entity.Property(e => e.AbncallsAsa).HasColumnName("abncallsASA");
            entity.Property(e => e.Abntime).HasColumnName("abntime");
            entity.Property(e => e.AbntimeAsa).HasColumnName("abntimeASA");
            entity.Property(e => e.Acdcalls).HasColumnName("acdcalls");
            entity.Property(e => e.AcdcallsAsa).HasColumnName("acdcallsASA");
            entity.Property(e => e.Acdtime).HasColumnName("acdtime");
            entity.Property(e => e.Acwtime).HasColumnName("acwtime");
            entity.Property(e => e.Anstime).HasColumnName("anstime");
            entity.Property(e => e.AnstimeAsa).HasColumnName("anstimeASA");
            entity.Property(e => e.Callbacks).HasColumnName("callbacks");
            entity.Property(e => e.Callsoffered).HasColumnName("callsoffered");
            entity.Property(e => e.CmsEquivalent).HasColumnName("CMS_Equivalent");
            entity.Property(e => e.DistinctAgent).HasColumnName("distinct_agent");
            entity.Property(e => e.EDate).HasColumnName("eDate");
            entity.Property(e => e.Et)
                .HasColumnType("datetime")
                .HasColumnName("et");
            entity.Property(e => e.Holdcalls).HasColumnName("holdcalls");
            entity.Property(e => e.Holdtime).HasColumnName("holdtime");
            entity.Property(e => e.Initiationmethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initiationmethod");
            entity.Property(e => e.Interval)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("interval");
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
            entity.Property(e => e.Maxanstime).HasColumnName("maxanstime");
            entity.Property(e => e.QueueName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("queue_name");
            entity.Property(e => e.Voicemails).HasColumnName("voicemails");
        });

        modelBuilder.Entity<HsplitdataOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("hsplitdata_old");

            entity.Property(e => e.Abncalls).HasColumnName("abncalls");
            entity.Property(e => e.Abntime).HasColumnName("abntime");
            entity.Property(e => e.Acdcalls).HasColumnName("acdcalls");
            entity.Property(e => e.Acdtime).HasColumnName("acdtime");
            entity.Property(e => e.Acwtime).HasColumnName("acwtime");
            entity.Property(e => e.Anstime).HasColumnName("anstime");
            entity.Property(e => e.Callbacks).HasColumnName("callbacks");
            entity.Property(e => e.Callsoffered).HasColumnName("callsoffered");
            entity.Property(e => e.CmsEquivalent).HasColumnName("CMS_Equivalent");
            entity.Property(e => e.DistinctAgent).HasColumnName("distinct_agent");
            entity.Property(e => e.EDate).HasColumnName("eDate");
            entity.Property(e => e.Et)
                .HasColumnType("datetime")
                .HasColumnName("et");
            entity.Property(e => e.Holdcalls).HasColumnName("holdcalls");
            entity.Property(e => e.Holdtime).HasColumnName("holdtime");
            entity.Property(e => e.Initiationmethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("initiationmethod");
            entity.Property(e => e.Interval)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("interval");
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
            entity.Property(e => e.QueueName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("queue_name");
            entity.Property(e => e.Voicemails).HasColumnName("voicemails");
        });

        modelBuilder.Entity<Hsplitdatum>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("hsplitdata");

            entity.Property(e => e.Abncalls).HasColumnName("abncalls");
            entity.Property(e => e.AbncallsAsa).HasColumnName("abncallsASA");
            entity.Property(e => e.Abntime).HasColumnName("abntime");
            entity.Property(e => e.AbntimeAsa).HasColumnName("abntimeASA");
            entity.Property(e => e.Acdcalls).HasColumnName("acdcalls");
            entity.Property(e => e.AcdcallsAsa).HasColumnName("acdcallsASA");
            entity.Property(e => e.Acdtime).HasColumnName("acdtime");
            entity.Property(e => e.Acwtime).HasColumnName("acwtime");
            entity.Property(e => e.Anstime).HasColumnName("anstime");
            entity.Property(e => e.AnstimeAsa).HasColumnName("anstimeASA");
            entity.Property(e => e.Callbacks).HasColumnName("callbacks");
            entity.Property(e => e.Callsoffered).HasColumnName("callsoffered");
            entity.Property(e => e.CmsEquivalent).HasColumnName("CMS_Equivalent");
            entity.Property(e => e.DistinctAgent).HasColumnName("distinct_agent");
            entity.Property(e => e.Edate).HasColumnName("edate");
            entity.Property(e => e.Et)
                .HasColumnType("datetime")
                .HasColumnName("et");
            entity.Property(e => e.Holdcalls).HasColumnName("holdcalls");
            entity.Property(e => e.Holdtime).HasColumnName("holdtime");
            entity.Property(e => e.Initiationmethod)
                .HasMaxLength(50)
                .HasColumnName("initiationmethod");
            entity.Property(e => e.Interval).HasColumnName("interval");
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
            entity.Property(e => e.QueueName)
                .HasMaxLength(50)
                .HasColumnName("queue_name");
            entity.Property(e => e.Voicemails).HasColumnName("voicemails");
        });

        modelBuilder.Entity<RocCallbackToolUpdate>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("RocCallbackToolUpdate");

            entity.Property(e => e.UpdateId)
                .ValueGeneratedOnAdd()
                .HasColumnName("Update_ID");
            entity.Property(e => e.UpdateTime)
                .HasColumnType("datetime")
                .HasColumnName("Update_Time");
        });

        modelBuilder.Entity<TblAwsProfile>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblAwsProfile");

            entity.Property(e => e.Coi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COI");
            entity.Property(e => e.Ncis).HasColumnName("NCIS");
            entity.Property(e => e.Nnpi).HasColumnName("NNPI");
            entity.Property(e => e.QueueUpdate).HasColumnType("datetime");
            entity.Property(e => e.RoutingProfile)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TemplateDesc)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Vip).HasColumnName("VIP");
        });

        modelBuilder.Entity<TblSplitDef>(entity =>
        {
            entity.ToTable("tblSplitDef");

            entity.Property(e => e.TblSplitDefId).HasColumnName("tblSplitDefID");
            entity.Property(e => e.Bucket).HasMaxLength(255);
            entity.Property(e => e.CallGroup)
                .HasMaxLength(255)
                .HasColumnName("Call_Group");
            entity.Property(e => e.CallGroupSpecial).HasMaxLength(255);
            entity.Property(e => e.CmsEquivalent).HasColumnName("CMS_Equivalent");
            entity.Property(e => e.Coi)
                .HasMaxLength(255)
                .HasColumnName("COI");
            entity.Property(e => e.Definition).HasMaxLength(255);
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
        });

        modelBuilder.Entity<TblSplitDefOld>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblSplitDef.old");

            entity.Property(e => e.Bucket)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.CallGroup)
                .HasMaxLength(50)
                .HasColumnName("Call_Group");
            entity.Property(e => e.CallGroupSpecial).HasMaxLength(50);
            entity.Property(e => e.CmsEquivalent).HasColumnName("CMS_Equivalent");
            entity.Property(e => e.Coi)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COI");
            entity.Property(e => e.Definition).HasMaxLength(50);
            entity.Property(e => e.IsAsaimpacting).HasColumnName("isASAImpacting");
            entity.Property(e => e.IsReportingWeb).HasColumnName("isReportingWeb");
            entity.Property(e => e.TblSplitDefId)
                .ValueGeneratedOnAdd()
                .HasColumnName("tblSplitDefID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
