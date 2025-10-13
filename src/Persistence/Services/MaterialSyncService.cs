using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Unit = MediatR.Unit;

namespace ContractorBackend.Persistence.Services
{
    public class MaterialSyncService<TEntity> : IMaterialSyncService<TEntity> where TEntity : class
    {
        private IServiceScopeFactory _scopeFactory;
        private IMapper _mapper;
        private IApplicationDbContext _dbContext;
        private IsSuiteClientService _isSuitHttp;


        public MaterialSyncService(
             IServiceScopeFactory scopeFactory
            )
        {
            _scopeFactory = scopeFactory;

            var scope = _scopeFactory.CreateScope();
            _mapper = scope.ServiceProvider.GetService<IMapper>();
            _dbContext = scope.ServiceProvider.GetService<IApplicationDbContext>();
            _isSuitHttp = scope.ServiceProvider.GetService<IsSuiteClientService>();

        }

        //public async void SyncDataMaterial()
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataMaterial)}");
        //    var queryParams = Utilities.GetIsSuiteMaxLimitOffsetParams();

        //    var isResult = await _isSuitHttp.GetAvailableMaterials(queryParams);

        //    if (isResult.Items.Any())
        //    {
        //        var list = _mapper.Map<List<TEntity>>(isResult.Items);

        //        await SaveToDbOnRefreshButton(list, nameof(Material.IsSuiteId));
        //    }
        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataMaterial)}");
        //}

        //public async void SyncDataConsumeMaterial(DateTime fromDate, DateTime toDate)
        //{
        //    Log.Warning($"Hangfire Background Job Started for {nameof(SyncDataConsumeMaterial)}");

        //    var fromYear = fromDate.GetPersianYear();
        //    var fromMonth = fromDate.GetPersianMonth().ToString("00");
        //    var fromYearMonth = $"{fromYear}{fromMonth}";

        //    var toYear = toDate.GetPersianYear();
        //    var toMonth = toDate.GetPersianMonth().ToString("00");
        //    var toYearMonth = $"{toYear}{toMonth}";

        //    var queryParams = new List<QueryParamModel>(){
        //        new QueryParamModel  { ParameterName  =  "P_DAT_FROM" , ParameterValue=fromYearMonth },
        //        new QueryParamModel  { ParameterName  =  "P_DAT_TO" , ParameterValue=toYearMonth}
        //    };
        //    queryParams = queryParams.Concat(Utilities.GetIsSuiteMaxLimitOffsetParams()).ToList();

        //    var isResult = await _isSuitHttp.GetMaterialConsumptions(queryParams);

        //    if (isResult.Items.Any())
        //    {
        //        var list = isResult.Items.Select(x => _mapper.Map<ConsumeMaterial>(x)).ToList();

        //        await UpSertConsumeMaterial(list);
        //    }
        //    Log.Warning($"Hangfire Background Job Ended for {nameof(SyncDataConsumeMaterial)}");
        //}


        //private Task<bool> UpSertConsumeMaterial(List<ConsumeMaterial> list)
        //{
        //    var strategy = _dbContext.Database.CreateExecutionStrategy();
        //    strategy.Execute(() =>
        //    {
        //        using (var transaction = _dbContext.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var fromApi = list.ToHashSet();

        //                var fromDb = _dbContext.ConsumeMaterials.AsNoTracking().ToList();
        //                var existIdsInDBIds = fromDb
        //                    .Select(x => new
        //                    {
        //                        ExpenseItemCode = x.ExpenseItemCode,
        //                        CostCenterCode = x.CostCenterCode,
        //                        Date = x.Date,
        //                    })
        //                    .ToList();

        //                #region Insert
        //                var toInsert = fromApi
        //                    .Where(x => existIdsInDBIds.All(l => l.ExpenseItemCode != x.ExpenseItemCode || l.CostCenterCode != x.CostCenterCode || l.Date != x.Date))
        //                    .ToList();

        //                foreach (var item in toInsert)
        //                {
        //                    item.GetType().GetProperty("Id").SetValue(item, null);
        //                }
        //                _dbContext.Set<ConsumeMaterial>().AddRange(toInsert);
        //                _dbContext.SaveChanges();
        //                #endregion


        //                #region Update
        //                var toUpdate = fromApi
        //                    .Where(x => existIdsInDBIds.All(l => l.ExpenseItemCode != x.ExpenseItemCode || l.CostCenterCode != x.CostCenterCode || l.Date != x.Date))
        //                    .ToList();

        //                foreach (var item in toUpdate)
        //                {
        //                    var entity = fromDb.FirstOrDefault(x =>
        //                                                    item.ExpenseItemCode != x.ExpenseItemCode ||
        //                                                    item.CostCenterCode != x.CostCenterCode ||
        //                                                    item.Date != x.Date
        //                                                    );

        //                    if (entity != null)
        //                    {
        //                        _mapper.Map(item, entity);
        //                        _dbContext.ConsumeMaterials.Update(entity as ConsumeMaterial);
        //                    }
        //                    else
        //                    {
        //                        // record is in api but not in db
        //                        // this record is handled in insert Region
        //                    }
        //                }
        //                _dbContext.SaveChanges();
        //                #endregion


        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        //                Log.Warning($"Hangfire Background Job Exception Eccured {nameof(UpSertConsumeMaterial)},With Error Mesage {errorMessage}");
        //                throw;
        //            }
        //            transaction.Dispose();
        //        }
        //    });


        //    return Task.FromResult(true);

        //}







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
