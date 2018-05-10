using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace ProjectAPI.Controllers
{
    //[RoutePrefix("api")]
    public class HomeController : ApiController
    {
        //static FakeDataSource Data = new FakeDataSource();

        public IHttpActionResult Get()
        {
            return CityNameMissingError();
        }
        [Route("{cityName}")]
        public IHttpActionResult Get(string cityName)
        {

            if (string.IsNullOrEmpty(cityName))
            {
                return CityNameMissingError();
            }

            var cd = "sdf";

            //var cd = Data.cityData.FirstOrDefault(c =>
            //  c.Name.ToLowerInvariant() == cityName.ToLowerInvariant() || c.NameInGeorgian == cityName);
            //if (cd == null)
            //    return new ResponseMessageResult(new HttpResponseMessage()
            //    {
            //        Content = new StringContent(
            //      $"City with name:{cityName} was not found in data source. Available city names are:{Environment.NewLine}{string.Join(",", Data.cityData.Select(c => c.Name).ToArray())}{Environment.NewLine}or:{Environment.NewLine}{string.Join(",", Data.cityData.Select(c => c.NameInGeorgian).ToArray())}")
            //    });
            return new JsonResult<string>(cd, new JsonSerializerSettings(), Encoding.UTF8, this);
        }

        private static IHttpActionResult CityNameMissingError()
        {
            return new ResponseMessageResult(new HttpResponseMessage()
            {
                Content = new StringContent("City name should be provided!")
            });
        }
    }

    //public class FakeDataSource
    //{
    //    public FakeDataSource()
    //    {
    //        foreach (var line in data.Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)))
    //        {
    //            var splitLine = line.Split('\t');
    //            var cd = new CityData();
    //            cd.Id = int.Parse(splitLine[0].Replace(".", ""));
    //            cd.Name = splitLine[1];
    //            cd.NameInGeorgian = splitLine[2];
    //            cd.Population1989 = int.Parse(splitLine[3].Replace(",", ""));
    //            cd.Population2002 = int.Parse(splitLine[4].Replace(",", ""));
    //            cd.Population2014 = int.Parse(splitLine[5].Replace(",", ""));
    //            cd.AdministrativeRegion = splitLine[6].Trim();
    //            cityData.Add(cd);
    //        }
    //    }



    //    // Define other methods and classes here

    //}
    public enum StatementState
    {
        Undefined = 0,
        Valid = 1,
        PendingInvalid = 2,
        Invalid = 3,
        PendingValid = 4,
    }
    public class StatementStatusUpdater
    {
        public static void RequestUpdateStatus(Statement statement, bool valid)
        {
            if (statement.State == StatementState.Valid && valid)
                throw new InvalidOperationException($"{nameof(statement)} already marked as {nameof(StatementState.Valid)}");
            if (statement.State == StatementState.Invalid && !valid)
                throw new InvalidOperationException($"{nameof(statement)} already marked as {nameof(StatementState.Invalid)}");

            var targetStatus = valid ? StatementState.PendingValid : StatementState.PendingInvalid;

            ChangeStatus(statement, targetStatus);
        }

        public static void SetValidStatus(Statement statement, bool valid)
        {
            if (statement.State == StatementState.Valid && valid)
                throw new InvalidOperationException($"{nameof(statement)}:{statement.Name} already marked as {nameof(StatementState.Valid)}");
            if (statement.State == StatementState.Invalid && !valid)
                throw new InvalidOperationException($"{nameof(statement)}:{statement.Name} already marked as {nameof(StatementState.Invalid)}");

            var targetStatus = valid ? StatementState.Valid : StatementState.Invalid;

            ChangeStatus(statement, targetStatus);
        }

        //public static IEnumerable<Statement> GetUserNamesWhichShouldBeInactive(Session uow)
        //{
        //    var allUsers = uow.Query<Statement>().Where(u => u.ExpiredOn == null);

        //    return allUsers
        //        .Where(u => u.State == StatementState.PendingInvalid ||
        //                    u.State == StatementState.Invalid);
        //}
        ////TODO: Poorly designed method
        //public static void TheseUsersShouldBeInactiveForNow(Statement[] inactiveOrPendingInactiveUsers, Session uow, int configId)
        //{
        //    if (inactiveOrPendingInactiveUsers.Any(u => u.State != StatementState.PendingInvalid && u.State != StatementState.Invalid))
        //        throw new InvalidOperationException($"Argument {nameof(inactiveOrPendingInactiveUsers)} mustn't contain users with status other than {nameof(StatementState.Invalid)} or {nameof(StatementState.PendingInvalid)}");
        //    //Filter out inactiveOrPendingInactiveUsers which already inactive
        //    var pendingInactiveusers = inactiveOrPendingInactiveUsers
        //        .Where(u => u.State != StatementState.Invalid).ToList();

        //    var requestCreationDate = DateTime.Now;

        //    foreach (var pi in pendingInactiveusers)
        //        RequestDeactivation(uow, configId, requestCreationDate, pi);

        //    //Request activation of inactiveOrPendingInactiveUsers which still inactive (in agent) but no longer marked inactive (in interface)
        //    var pendingActiveusers = uow.Query<Statement>().Where(u => u.ExpiredOn == null && u.State == StatementState.PendingValid);

        //    //var ids = inactiveOrPendingInactiveUsers.Select(u => u.Oid).ToList();

        //    //var pendingActiveusers = uow.Query<Statement>() allUsers.Where(u => !ids.Contains(u.Oid) && u.State != StatementState.Valid).ToList();

        //    foreach (var pa in pendingActiveusers)
        //        RequestActivation(uow, configId, requestCreationDate, pa);
        //}

        //public static void SetInitialStatusToNewUsers(Session session, Action<Exception> exceptionLogger)
        //{
        //    var users = session.Query<Statement>().Where(u => u.State == null || u.State == StatementState.Undefined);
        //    foreach (var user in users)
        //    {
        //        SetInitialStatus(user)
        //            .OnFailure(er => exceptionLogger.Invoke(new Exception(er)));
        //    }
        //}

        //public static void ApplyActionsAndRemoveRequests(Session uow, Action<Exception> exceptionLogger)
        //{
        //    var requests = uow.Query<StatementStateUpdateRequest>();
        //    foreach (var req in requests)
        //        ApplyRequestAndRemoveIfCompleted(req, uow, exceptionLogger);


        //}

        public static bool AgentConfigurationShouldBeUpdated(Session uow, string agentUpdateFilePath)
        {
            var anyAgentProfileGroupHasChanged = uow.Query<AgentProfileGroup.AgentProfileGroup>()
                                                     .Count(apg => apg.IsAccountedForInAgentConfig) > 0;

            return anyAgentProfileGroupHasChanged || MonitoringStatusUpdatedForUsers(uow) || LastAgentUpdateFileVersionChanged(agentUpdateFilePath);
        }

        private static bool LastAgentUpdateFileVersionChanged(string agentUpdateFilePath)
        {
            var fileExists = System.IO.File.Exists(agentUpdateFilePath);
            if (!fileExists) return false;
            string lastVersionKnown = null;
            using (var uow = new UnitOfWork())
            {
                var lastXml = uow.Query<AgentConfigurationSerializationLog>().OrderBy(l => l.ID).LastOrDefault();
                if (lastXml == null) return true;

                lastVersionKnown = lastXml.CurrentAgentVersion;
                if (lastVersionKnown == null) return true;
            }

            var currentFileVersion = FileVersionInfo.GetVersionInfo(agentUpdateFilePath)?.FileVersion;

            return lastVersionKnown != currentFileVersion;
        }

        public static void ClearIsAccountedForInAgentConfig(Session uow)
        {
            foreach (var instance in uow.Query<AgentProfileGroup.AgentProfileGroup>().Where(a => a.IsAccountedForInAgentConfig))
                instance.IsAccountedForInAgentConfig = false;
        }

        public static Result SetInitialStatus(Statement user)
        {
            return GetUpdatedIsInactiveStatus(user)
                .OnSuccess(status =>
                {
                    if (status.HasValue)
                        SetValidStatus(statement: user,
                            valid: !status.Value.IsInactive);
                    else ChangeStatus(user, StatementState.Undefined, force: true);
                });
        }

        public static Result<Maybe<ActiveStatus>> GetUpdatedIsInactiveStatus(Statement user)
        {
            using (var uow = new UnitOfWork())
            {
                var lastLogByThisUser = uow.Query<UserSession>().Where(us => us.Statement.Oid == user.Oid)
                    .OrderByDescending(us => us.SaveDate)
                    .FirstOrDefault();
                if (lastLogByThisUser == null) return Result.Ok(Maybe<ActiveStatus>.From(null));
                // return Result.Fail<ActiveStatus>($"Can't determine user monitoring status because logs of that user:{user?.Name} missing");

                return Result.Ok(Maybe<ActiveStatus>.From(new ActiveStatus(lastLogByThisUser.IsInactive,
                    lastLogByThisUser.MaxConfigurationId)));
            }
        }

        public sealed class ActiveStatus
        {
            public ActiveStatus(bool isInactive, int configId)
            {
                IsInactive = isInactive;
                ConfigId = configId;
            }

            public bool IsInactive { get; }
            public int ConfigId { get; }
        }

        static bool MonitoringStatusUpdatedForUsers(Session uow)
        {
            var pendingActiveusers = uow.Query<Statement>()
                .Where(su => su.State == StatementState.PendingValid);

            var pendingInactiveusers = uow.Query<Statement>()
                .Where(su => su.State == StatementState.PendingInvalid);

            var currentlyRegisteredRequests = uow.Query<StatementStateUpdateRequest>();

            //Users which have any pending status but update has not been requested yet
            var newPendingActives =
                pendingActiveusers.Where(
                    pau => !currentlyRegisteredRequests.Any(req => req.PendingActive && req.UserId == pau.Oid));
            var newPendingInactives =
                pendingInactiveusers.Where(
                    pau => !currentlyRegisteredRequests.Any(req => !req.PendingActive && req.UserId == pau.Oid));
            //
            return newPendingInactives.Any() || newPendingActives.Any();
        }

        static void ApplyRequestAndRemoveIfCompleted(StatementStateUpdateRequest req, Session session, Action<Exception> exceptionLogger)
        {
            var user = session.Query<Statement>().FirstOrDefault(u => u.Oid == req.UserId);
            if (user == null)
            {
                req.Delete();
                return;
            }
            GetUpdatedIsInactiveStatus(user)
                .OnSuccess(lastUserConfigAndStatus =>
                {
                    if (!lastUserConfigAndStatus.HasValue) return;
                    var isInactive = lastUserConfigAndStatus.Value.IsInactive;
                    var lastConfigReadByUser = lastUserConfigAndStatus.Value.ConfigId;

                    if (lastConfigReadByUser < req.ConfigVersion) return;

                    // If Got inactive
                    if (isInactive && !req.PendingActive)
                    {
                        if (user.State != StatementState.Invalid)
                            SetValidStatus(user, false);
                        req.Delete();
                        return;
                    }
                    // If Got active
                    if (!isInactive && req.PendingActive)
                    {
                        if (user.State != StatementState.Valid)
                            SetValidStatus(user, true);
                        req.Delete();
                    }
                })
                .OnFailure(er => exceptionLogger.Invoke(new Exception(er)))
                ;
        }
        static void UpdateUserStatusWithoutRequest(Statement user, Action<Exception> exceptionLogger)
        {
            GetUpdatedIsInactiveStatus(user)
                .OnSuccess(lastUserConfigAndStatus =>
                {
                    if (!lastUserConfigAndStatus.HasValue)
                    {
                        SetValidStatus(user, false);
                        return;
                    }
                    var isInactive = lastUserConfigAndStatus.Value.IsInactive;

                    if (isInactive)
                    {
                        if (user.State != StatementState.Invalid)
                            SetValidStatus(user, false);
                    }
                    else
                    {
                        if (user.State != StatementState.Valid)
                            SetValidStatus(user, true);
                    }
                })
                .OnFailure(er => exceptionLogger.Invoke(new Exception(er)))
                ;
        }


        static void RequestActivation(Session uow, int configId, DateTime requestCreationDate, Statement pi)
        {
            new StatementStateUpdateRequest(uow)
            {
                ConfigVersion = configId,
                LastModified = requestCreationDate,
                PendingActive = true,
                UserId = pi.Oid
            };
        }

        static void RequestDeactivation(Session uow, int configId, DateTime requestCreationDate, Statement pi)
        {
            new StatementStateUpdateRequest(uow)
            {
                ConfigVersion = configId,
                LastModified = requestCreationDate,
                PendingActive = false,
                UserId = pi.Oid
            };
        }



        static void ChangeStatus(Statement user, StatementState targetStatus, bool force = false)
        {
            if (!force)
            {
                try
                {
                    EnsureValidStatusChange(user.State, targetStatus);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException($"Couldn't change status of user:{user.Name} current status:{user.State} to {targetStatus}. See inner exception for details", e);
                }
            }

            user.State = targetStatus;
        }

        static void EnsureValidStatusChange(StatementState? currentStatus, StatementState targetStatus)
        {
            if (targetStatus == StatementState.Undefined)
                throw new InvalidOperationException($"Target status:{targetStatus} is invalid");

            //Undefined status can be changed (only) to active or inactive
            if (currentStatus == StatementState.Undefined)
            {
                if (StatusIsValidForInitialization(targetStatus)) return;
            }
            else if (currentStatus == null)
            {
                if (StatusIsValidForInitialization(targetStatus)) return;
                throw new InvalidOperationException(
                    $"Uninitialized status can only be initialized with following statuses: \"{StatementState.Valid}\",\"{StatementState.Invalid}\"");
            }
            //

            var availableStatuses = Enum.GetValues(typeof(StatementState)).OfType<StatementState>();

            //0 indexed enumerable
            var assignableStatuses = availableStatuses.Where(s => s != StatementState.Undefined);

            var numberOfAssgnableStatuses = assignableStatuses.Count();

            var currentStatusId = (int)currentStatus;

            //Status id cant be 0 
            var nextStatusId = currentStatusId + 1;
            if (nextStatusId > numberOfAssgnableStatuses)
                nextStatusId = 1;

            //karoche wreze midis ra

            var nextStatus = (StatementState)nextStatusId;

            if (targetStatus != nextStatus)
                throw new InvalidOperationException(
                    $"Only status:{nextStatus} can be applied after current status:{currentStatus}");
            //Everything's Ok. Phew!

        }

        static bool StatusIsValidForInitialization(StatementState targetStatus)
        {
            if (targetStatus == StatementState.Valid || targetStatus == StatementState.Invalid)
                return true;
            return false;
        }

        //public static void UpdateStatusesWithoutRequests(UnitOfWork uow, Action<Exception> exceptionLogger)
        //{
        //    var pendingActiveUsers = uow.Query<Statement>().Where(u => u.ExpiredOn == null && u.State == StatementState.PendingValid).ToList()
        //        .Where(u => !uow.Query<StatementStateUpdateRequest>().Any(r => r.UserId == u.Oid));
        //    var pendingInactiveUsers = uow.Query<Statement>().Where(u => u.ExpiredOn == null && u.State == StatementState.PendingInvalid).ToList()
        //        .Where(u => !uow.Query<StatementStateUpdateRequest>().Any(r => r.UserId == u.Oid));

        //    foreach (var pa in pendingActiveUsers)
        //        UpdateUserStatusWithoutRequest(pa, exceptionLogger);

        //    foreach (var pi in pendingInactiveUsers)
        //        UpdateUserStatusWithoutRequest(pi, exceptionLogger);
        //}
    }

    public class Statement
    {
        //[Key(true)]
        public Guid Oid { get; set; }
        public StatementState State { get; set; }
        public string Name { get; set; }
    }
    public class StatementStateUpdateRequest
    {

        public StatementStateUpdateRequest(Session session)// : base(session)
        {
        }
        //[Key(true)]
        public Guid Oid { get; set; }
        public Guid UserId { get; set; }
        public bool PendingActive { get; set; }
        public int ConfigVersion { get; set; }
        public DateTime LastModified { get; set; }

    }

    public class Session
    {
    }
}
