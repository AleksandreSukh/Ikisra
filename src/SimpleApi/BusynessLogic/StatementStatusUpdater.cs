using System;
using System.Linq;

namespace ProjectAPI.Controllers
{
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
}