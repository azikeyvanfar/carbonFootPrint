using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ContractorBackend.Persistence.Services
{
    public class HangFireSyncService<TEntity> : IHangFireSyncService<TEntity> where TEntity : class
    {
        private IServiceScopeFactory _scopeFactory;
        private IMapper _mapper;
        private IApplicationDbContext _dbContext;
        private IsSuiteClientService _isSuitHttp;


        public HangFireSyncService(
             IServiceScopeFactory scopeFactory
            )
        {
            _scopeFactory = scopeFactory;

            var scope = _scopeFactory.CreateScope();
            _mapper = scope.ServiceProvider.GetService<IMapper>();
            _dbContext = scope.ServiceProvider.GetService<IApplicationDbContext>();
            _isSuitHttp = scope.ServiceProvider.GetService<IsSuiteClientService>();

        }

        //public async Task<bool> SyncDataBusinessUnit()
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataBusinessUnit)}");
        //    var queryParams = Utilities.GetIsSuiteMaxLimitOffsetParams();

        //    var isResult = await _isSuitHttp.GetOjcPersonnelBusinessUnitsViewAsync(queryParams);

        //    if (isResult.Items.Any())
        //    {
        //        var list = _mapper.Map<List<TEntity>>(isResult.Items);

        //        await SaveToDbOnRefreshButton(list, nameof(BusinessUnit.IsSuiteId), false);
        //    }
        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataBusinessUnit)}");

        //    return true;
        //}

        //public async Task<bool> SyncDataArea()
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataArea)}");
        //    var queryParams = Utilities.GetIsSuiteMaxLimitOffsetParams();

        //    var isResult = await _isSuitHttp.GetOjcAreasViewAsync(queryParams);

        //    if (isResult.Items.Any())
        //    {
        //        var list = _mapper.Map<List<TEntity>>(isResult.Items);

        //        await SaveToDbOnRefreshButton(list, nameof(Area.IsSuiteId), false);
        //    }
        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataArea)}");

        //    return true;
        //}

        //public async Task<bool> SyncDataUserCalendar()
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataUserCalendar)}");
        //    var queryParams = Utilities.GetIsSuiteMaxLimitOffsetParams();

        //    var isResult = await _isSuitHttp.GetUserCalendarViewAsync(queryParams);

        //    if (isResult.Items.Any())
        //    {
        //        var list = _mapper.Map<List<TEntity>>(isResult.Items);

        //        await SaveToDbOnRefreshButton(list, nameof(UserCalendar.IsSuiteId));
        //    }
        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataUserCalendar)}");

        //    return true;
        //}


        //public async Task<bool> SyncDataCommittee()
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataCommittee)}");
        //    var queryParams = Utilities.GetIsSuiteMaxLimitOffsetParams();

        //    var body = new
        //    {
        //        chartCode = "EVL",
        //        status = "ACTIVE"
        //    };
        //    var isResult = await _isSuitHttp.GetBPMSCommitteeTreeViewAsync(queryParams, body);

        //    if (isResult.Any())
        //    {
        //        var list = _mapper.Map<List<TEntity>>(isResult);

        //        await SaveToDbOnRefreshButton(list, nameof(Committee.IsSuiteId));
        //    }
        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataCommittee)}");

        //    return true;
        //}
        //public async Task<bool> SyncDataContractor()
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataContractor)}");
        //    var queryParams = Utilities.GetIsSuiteMaxLimitOffsetParams();

        //    var isResult = await _isSuitHttp.GetCpmHsewebContractsViewAsync(queryParams);

        //    if (isResult.Items.Any())
        //    {
        //        var list = _mapper.Map<List<TEntity>>(isResult.Items);

        //        await SaveToDbOnRefreshButton(list, nameof(Contractor.ContractorContractNumber), false);
        //    }
        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataContractor)}");

        //    return true;
        //}

        //public async Task<bool> SyncDataEmployee()
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataEmployee)}");

        //    var queryParams = Utilities.GetIsSuiteMaxLimitOffsetParams();

        //    var isResult = await _isSuitHttp.GetAllEmployeeViewAsync(queryParams);

        //    if (isResult.Items.Any())
        //    {
        //        var list = _mapper.Map<List<TEntity>>(isResult.Items);

        //        await SaveToDbOnRefreshButton(list, nameof(Employee.PersonnelCode), false);
        //    }



        //    //var limit = 1_000;
        //    //var offset = 0;
        //    //var hasMore = false;
        //    //var isResult = new IsSuiteResponse<EmployeeVM>();
        //    //isResult.Items = new();

        //    //do
        //    //{
        //    //    var queryParams = new List<QueryParamModel>()     {
        //    //            new QueryParamModel  { ParameterName  =  "limit" ,ParameterValue= limit .ToString()},
        //    //            new QueryParamModel  { ParameterName  =  "offset" ,ParameterValue= offset.ToString()}
        //    //        };

        //    //    var partialResult = await _isSuitHttp.GetAllEmployeeViewAsync(queryParams);
        //    //    if (partialResult.Items.Any())
        //    //    {
        //    //        isResult.Items.AddRange(partialResult.Items);
        //    //    }
        //    //    hasMore = partialResult.HasMore;
        //    //    offset += limit;
        //    //}
        //    //while (hasMore);

        //    //if (isResult.Items.Any())
        //    //{
        //    //    var list = _mapper.Map<List<TEntity>>(isResult.Items);
        //    //    await SaveToDbOnRefreshButton(list, nameof(Employee.PersonnelCode));
        //    //}



        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataEmployee)}");

        //    return true;
        //}





        /// <summary>
        /// Save to DB
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isSuiteIdProperty">name of is-suite id column</param>
        /// <param name="softDelete">deleted record : make soft delete OR IsActive=false</param>
        /// <returns></returns>
        private async Task<Unit> SaveToDbOnRefreshButton(List<TEntity> list, string isSuiteIdProperty, bool softDelete = true)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var fromApi = list.ToHashSet();

                        var fromDb = _dbContext.Set<TEntity>().AsNoTracking().ToList();
                        var existIdsInDBIds = fromDb.Select(x => x.GetType()?.GetProperty(isSuiteIdProperty)?.GetValue(x, null) ?? null).ToList();


                        #region Insert
                        var toInsert = fromApi.Where(x => existIdsInDBIds.All(l => l?.ToString() != x.GetType()?.GetProperty(isSuiteIdProperty)?.GetValue(x, null)?.ToString())).ToList();
                        foreach (var item in toInsert)
                        {
                            item.GetType().GetProperty("Id").SetValue(item, null);
                        }
                        _dbContext.Set<TEntity>().AddRange(toInsert);
                        _dbContext.SaveChanges();
                        #endregion


                        #region Update
                        var toUpdate = fromApi.Where(x => existIdsInDBIds.Any(l => l.ToString() != x.GetType()?.GetProperty(isSuiteIdProperty)?.GetValue(x, null)?.ToString()))?.ToList();
                        foreach (var item in toUpdate)
                        {
                            var entity = fromDb.FirstOrDefault(x =>
                                                x.GetType()?
                                                .GetProperty(isSuiteIdProperty)?
                                                .GetValue(x)?.ToString()
                                                ==
                                                item.GetType()?
                                                .GetProperty(isSuiteIdProperty)?
                                                .GetValue(item)?.ToString()
                                                );

                            if (entity != null)
                            {
                                _mapper.Map(item, entity);
                                _dbContext.Set<TEntity>().Update(entity as TEntity);
                            }
                            else
                            {
                                // record is in api but not in db
                                // this record is handled in insert Region
                            }
                        }
                        _dbContext.SaveChanges();
                        #endregion


                        #region Delete
                        if (softDelete)
                        {
                            // todo: برای جلوگیری از خطای دیلیت برای سطر هایی که در دیگر جداول استفاده شدند احتمالا باید به جای سافت دیلیت از isActive=false
                            // استفاده کنیم 
                            // برای این کار باید همه دراپ داون ها بر اساس isActive لود شوند که غیرفعال ها لود نشوند
                            var toDelete = fromDb.Where(x =>
                                                    fromApi.All(l =>
                                                            l.GetType()?.GetProperty(isSuiteIdProperty)?.GetValue(l, null)?.ToString() !=
                                                            x.GetType()?.GetProperty(isSuiteIdProperty)?.GetValue(x, null)?.ToString()
                                                            )
                                                    )
                                                    .ToHashSet();
                            if (toDelete.Any())
                            {
                                _dbContext.Set<TEntity>().RemoveRange(toDelete);
                            }

                        }
                        else   // فقط غیرفعال میکند بجای سافت دیلیت
                        {
                            var toDelete = fromDb.Where(x =>
                                             fromApi.All(l =>
                                                     l.GetType()?.GetProperty(isSuiteIdProperty)?.GetValue(l, null)?.ToString() !=
                                                     x.GetType()?.GetProperty(isSuiteIdProperty)?.GetValue(x, null)?.ToString()
                                                     )
                                             )
                                             .ToHashSet();

                            if (toDelete.Any())
                            {
                                foreach (var item in toDelete)
                                {
                                    item.GetType().GetProperty("IsActive").SetValue(item, false);
                                }

                                _dbContext.Set<TEntity>().UpdateRange(toDelete);
                            }
                        }

                        _dbContext.SaveChanges();

                        #endregion


                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        Log.Warning($"Hangfire Background Job Exception Eccured {nameof(SaveToDbOnRefreshButton)},With Error Mesage {errorMessage}");
                        throw;
                    }
                    transaction.Dispose();
                }
            });


            return Unit.Value;

        }


    }

}
