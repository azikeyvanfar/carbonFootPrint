using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContractorBackend.Application.Common;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Persistence.Services
{
    public class SeedService : ISeedService
    {
        private readonly IApplicationDbContext _dbContext;
        public SeedService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SeedLookups()
        {

            await SeedLookupHarmfulFactorCategory();
            await SeedLookupDataCategory();
            await SeedLookupSymbol();
            await SeedLookupProbabilitySymbol();
            await SeedLookupIncidentShift();
            await SeedLookupInjurySpot();
            await SeedLookupInjuryResult();
            await SeedLookupIncidentReason();
            await SeedLookupIncidentType();
            await SeedLookupReviewReason();
            await SeedLookupIndicator();
            await SeedLookupOldCpmIndicator();

            return true;
        }
        public async Task<bool> SeedLookupHarmfulFactorCategory()
        {
            var code = "harmfulFactorCategory";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "دسته بندی عوامل زیان آور",
                Code = null,
                Description = "سردسته دسته بندی عوامل زیان آور",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "1", FaName = "فیزیکی", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "2", FaName = "مکانیکی", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "3", FaName = "ارگونومیکی", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "4", FaName = "شیمیایی", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5", FaName = "بیولوژیکی", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "6", FaName = "امنیتی", Code = code, Description = null, Type = LookupType.Item, Priority = 6, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "7", FaName = "بلایای طبیعی", Code = code, Description = null, Type = LookupType.Item, Priority = 7, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "8", FaName = "سایبری", Code = code, Description = null, Type = LookupType.Item, Priority = 8, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "9", FaName = "سایر", Code = code, Description = null, Type = LookupType.Item, Priority = 9, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupDataCategory()
        {
            var code = "dataCategory";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "دسته بندی داده ها",
                Code = null,
                Description = "سردسته دسته بندی داده ها",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "Severity", FaName = "تعیین شدت (S)", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = "Severity" });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "Probability", FaName = "تعیین احتمال وقوع (P)", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = "Probability" });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "ImpactOnRiskLevel", FaName = "تاثیر بر سطح ریسک", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = "ImpactOnRiskLevel" });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "Practicality", FaName = "کاربردی بودن", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = "Practicality" });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "ReturnInvestment", FaName = "هزینه /بازگشت سرمایه", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = "ReturnInvestment" });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupSymbol()
        {
            var code = "symbol";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "نماد",
                Code = null,
                Description = "سردسته نماد",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5", FaName = "5", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "4", FaName = "4", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "3", FaName = "3", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "2", FaName = "2", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "1", FaName = "1", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupProbabilitySymbol()
        {
            var code = "probabilitySymbol";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "نماداحتمال وقوع",
                Code = null,
                Description = "سردسته نماداحتمال وقوع",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "A", FaName = "A", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "B", FaName = "B", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "C", FaName = "C", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "D", FaName = "D", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "E", FaName = "E", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupIncidentShift()
        {
            var code = "incidentShift";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "شیفت وقوع حادثه",
                Code = null,
                Description = "سردسته شیفت وقوع حادثه",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5001", FaName = "شیفت 1 (8 ساعته)", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5002", FaName = "شیفت 2 (8 ساعته)", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5003", FaName = "شیفت 3 (8 ساعته)", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5004", FaName = "شیفت 1 (12 ساعته)", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5005", FaName = "شیفت 2 (12 ساعته)", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5006", FaName = "روزکار", Code = code, Description = null, Type = LookupType.Item, Priority = 6, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5007", FaName = "نوبت کار A", Code = code, Description = null, Type = LookupType.Item, Priority = 7, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "5008", FaName = "نوبت کار B", Code = code, Description = null, Type = LookupType.Item, Priority = 8, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupInjurySpot()
        {
            var code = "injurySpot";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "موضع اسیب",
                Code = null,
                Description = "سردسته موضع اسیب",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد1", FaName = "سر", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد2", FaName = "چشم", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد3", FaName = "گردن", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد4", FaName = "تنه", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد5", FaName = "اندام فوقانی", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد6", FaName = "اندام تهتانی", Code = code, Description = null, Type = LookupType.Item, Priority = 6, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد7", FaName = "ارکان های داخلی و احشا", Code = code, Description = null, Type = LookupType.Item, Priority = 7, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد8", FaName = "کمر و ستون فقرات", Code = code, Description = null, Type = LookupType.Item, Priority = 8, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد9", FaName = "ارگان های متعدد", Code = code, Description = null, Type = LookupType.Item, Priority = 9, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد10", FaName = "سایر", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupInjuryResult()
        {
            var code = "injuryResult";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "نوع یا نتیجه اسیب",
                Code = null,
                Description = "سردسته نوع یا نتیجه اسیب",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد1", FaName = "مرگ", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = "1" });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد2", FaName = "قطع عضو", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد3", FaName = "بریدگی و جراحت", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد4", FaName = "شکستگی ها و دررفتگی ها", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد5", FaName = "سوختگی", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد6", FaName = "برق گرفتگی و شوک الکتریکی", Code = code, Description = null, Type = LookupType.Item, Priority = 6, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupIncidentReason()
        {
            var code = "incidentReason";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "علت وقوع حادثه",
                Code = null,
                Description = "سردسته علت وقوع حادثه",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد1", FaName = "اعمال غیر ایمن", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد2", FaName = "شرایط غیر ایمن", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد3", FaName = "اعمال و شرایط غیر ایمن", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupIncidentType()
        {
            var code = "incidentType";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "نوع حادثه",
                Code = null,
                Description = "سردسته نوع حادثه",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد1", FaName = "برخورد , ضریه , تصادف با اجسام و وسایل در حال حرکت با موانع ثابت و اسیا در حال جابجایی", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد2", FaName = "انفجار", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد3", FaName = "اوار و تخریب", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد4", FaName = "سقوط از سطحی به سطح پایین تر", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد5", FaName = "برخورد با اجسام رها یا پرتاب شده", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد6", FaName = "گیر کردن بین دو جسم سخت", Code = code, Description = null, Type = LookupType.Item, Priority = 6, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد7", FaName = "افتادن روی سطح زمین", Code = code, Description = null, Type = LookupType.Item, Priority = 7, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد8", FaName = "قرار گرفتن در معرض حرارت بیش از حد", Code = code, Description = null, Type = LookupType.Item, Priority = 8, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد9", FaName = "قرار گرفتن در معرض جریان الکتریکی", Code = code, Description = null, Type = LookupType.Item, Priority = 9, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد10", FaName = "پاشش مواد مذاب و مواد شیمیایی", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد11", FaName = "تماس با اسیا تیز و برنده", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد12", FaName = "اشیا مواد داغ", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد13", FaName = "قرار گرفتن در معرض اشعه", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد14", FaName = "ورود اجسام و ذرات خارجی به چشم", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SeedLookupReviewReason()
        {
            var code = "reviewReason";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "دلیل بازنگری",
                Code = null,
                Description = "سردسته دلیل بازنگری",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد1", FaName = "تغییر در قوانین، مقررات و سایر الزامات", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد2", FaName = "تغییر در خط مشی سازمان", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد3", FaName = "تغییر در فرآیند تولید، تجهیز، محیط و شرایط کار", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد4", FaName = "تغییر در نتایج پایش ها، و اندازه گیري ها", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد5", FaName = "تغییرات تکنولوژیکی و دانشی", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد6", FaName = "انجام هرگونه اقدامات اصلاحی/ پیشگیرانه/ کنترلی", Code = code, Description = null, Type = LookupType.Item, Priority = 6, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد7", FaName = "نتایج به دست آمده از بررسی حوادث", Code = code, Description = null, Type = LookupType.Item, Priority = 7, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد8", FaName = "نتایج بدست آمده از بررسی شبه حوادث", Code = code, Description = null, Type = LookupType.Item, Priority = 8, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد9", FaName = "شناسایی خطرات جدید", Code = code, Description = null, Type = LookupType.Item, Priority = 9, CategoryId = parent.Id, EnumCode = Utilities.InitialReviewReason });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد10", FaName = "نتایج بازرسی ها و ارزیابی هاي ایمنی", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "کد11", FaName = "حداقل سالی یک بار", Code = code, Description = null, Type = LookupType.Item, Priority = 10, CategoryId = parent.Id, EnumCode = null });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }


        public async Task<bool> SeedLookupIndicator()
        {
            var code = "indicator";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "شاخص",
                Code = null,
                Description = "سردسته شاخص",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "H2O", FaName = "آب", Code = code, Description = null, Type = LookupType.Item, Priority = 1, CategoryId = parent.Id, EnumCode = Utilities.H2OEnumCode });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "CO2", FaName = "دی اکسید کربن", Code = code, Description = null, Type = LookupType.Item, Priority = 2, CategoryId = parent.Id, EnumCode = Utilities.CO2EnumCode });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "ENERGY", FaName = "انرژی", Code = code, Description = null, Type = LookupType.Item, Priority = 3, CategoryId = parent.Id, EnumCode = Utilities.EnergyEnumCode });

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }
        public async Task<bool> SeedLookupOldCpmIndicator()
        {
            var code = "oldCpmIndicator";
            if (_dbContext.Lookups.Any(x => x.Code == code))
            {
                return true;
            }

            List<Lookup> list = new();
            var parent = new Lookup
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                EnName = null,
                FaName = "شاخص سیستم قدیمی",
                Code = null,
                Description = "سردسته شاخص سیستم قدیمی",
                Type = LookupType.Lookup,
                Priority = 1,
                CategoryId = null,
                EnumCode = null
            };
            list.Add(parent);

            #region  Old Cpm 
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "SOX", FaName = "sox", Code = code, Description = null, Type = LookupType.Item, Priority = 4, CategoryId = parent.Id, EnumCode = "sox" });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "NOX", FaName = "nox", Code = code, Description = null, Type = LookupType.Item, Priority = 5, CategoryId = parent.Id, EnumCode = "nox" });
            list.Add(new Lookup { Id = Guid.NewGuid(), IsActive = true, EnName = "DUST", FaName = "dust", Code = code, Description = null, Type = LookupType.Item, Priority = 6, CategoryId = parent.Id, EnumCode = "dust" });
            #endregion

            _dbContext.Lookups.AddRange(list);

            await _dbContext.SaveChangesAsync();
            return true;

        }





        //public async Task<bool> SeedUnits()
        //{
        //    if (_dbContext.Units.Any())
        //    {
        //        return true;
        //    }

        //    List<Unit> list = new();

        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سانتیگراد(دما)", Code = "CG", Description = "سانتیگراد (دما)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "KL", Code = "KL", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تخته", Code = "PIL", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "دقیقه", Code = "MIN", Description = "MINUTE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "در میلیون", Code = "PPM", Description = "PART PER MILION" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "متر بر دقیقه", Code = "M", Description = "متر بر دقیقه" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "هزارتائي", Code = "TH", Description = "THOUSAND" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلو نرمال متر مکعب", Code = "17", Description = "1000 NORMAL CUBIC METER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نرمال متر مکعب", Code = "16", Description = "NORMAL CUBIC METER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تن 1000", Code = "22", Description = "1000 TON" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "وات  ساعت", Code = "35", Description = "1000 WATT HOURS" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "كالري", Code = "38", Description = "CALORY" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "كالريها1000", Code = "39", Description = "1000 CALORIES" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "كيسه", Code = "BG", Description = "BAG" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "بشكه", Code = "BL", Description = "BARREL" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "شاخه", Code = "BM", Description = "BEAM" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "جعبه", Code = "BX", Description = "BOX , PACKAGE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "بسته", Code = "CA", Description = "CASE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سانتيمترمكعب", Code = "CC", Description = "CUBIC CENTIMETRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "رول", Code = "CL", Description = "COIL" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سانتيمتر", Code = "CM", Description = "CENTIMETRE (= INCH 0/39)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "بطري                                          ,قوطي,شيشه", Code = "CN", Description = "CAN" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نسخه", Code = "CP", Description = "COPY" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "كارتن", Code = "CR", Description = "CARTON" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "روز", Code = "DD", Description = "DAY" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "دوجين                                           (دوازدهتائي)", Code = "DZ", Description = "DOZEN" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "گالنبريتانيا", Code = "GL", Description = "GALLON (LT 4.543)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ميليگرم", Code = "GM", Description = "MILIGRAM" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "گرم", Code = "GR", Description = "GRAM" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "قراص 144(تكه)", Code = "GS", Description = "GROSS (144 PRICES)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ساعت", Code = "HH", Description = "HOUR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "اينچ", Code = "IN", Description = "INCH (CM 2.54)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلو، مترمکعب  یا 1000 متر مکعب", Code = "KC", Description = "CUBIC KILOMETER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "كيلوگرم", Code = "KG", Description = "KILOGRAM (LB 2.205)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "كيلووات ساعت", Code = "KW", Description = "KILOWATT HOUR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "پوند", Code = "LB", Description = "POUND AV. (GR 453.6)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ميليليتر", Code = "LM", Description = "MILLILITRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مبلغ كل (براي قراردادهايخدماتي)", Code = "LS", Description = "LUMP SUM" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ليتر", Code = "LT", Description = "LITRE (GB GALLON 0.22)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مترمكعب", Code = "MC", Description = "CUBIC METER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ماشينساعت", Code = "MH", Description = "MACHINE HOUR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مايل", Code = "ML", Description = "MILE (KM 1.609)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ميليمتر", Code = "MM", Description = "MILLIMETRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ماه", Code = "MO", Description = "MONTH" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مترمربع", Code = "MS", Description = "SQUARE METRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "متر", Code = "MT", Description = "METRE (FT 3.29/YD 1.094)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "قالب", Code = "MU", Description = "MOULD" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مگاوات ساعت", Code = "MWH", Description = "MEGAWATT HOUR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ماشينسال", Code = "MY", Description = "MACHINE YEAR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "NUMBER-MILLIMETRE", Code = "NM", Description = "NUMBER-MILLIMETRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "عدد", Code = "NR", Description = "NUMBER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نفردوره", Code = "PC", Description = "PERSON COURSE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نفرروز", Code = "PD", Description = "PERSON DAY" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نفرساعت", Code = "PH", Description = "PERSON/HOUR OR MAN/HOUR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "قطعه", Code = "PI", Description = "PIECE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نفرماه", Code = "PM", Description = "PERSON MONTH" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "جفت", Code = "PR", Description = "PAIR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "حلقه", Code = "RG", Description = "RING" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "RIALS", Code = "RI", Description = "RIALS" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "توپ", Code = "RL", Description = "ROLL" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ورق", Code = "RM", Description = "REAM" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سري", Code = "SE", Description = "SERIES" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "دست", Code = "ST", Description = "SET" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تعرفهماهيانه", Code = "TM", Description = "MONTLY TARIFF" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تن", Code = "TN", Description = "TON (KG 1000)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سفر (رفت وبرگشت )", Code = "TT", Description = "TRAVEL (GO AND BACK)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "دستگاه", Code = "UN", Description = "UNIT" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "جلد", Code = "VM", Description = "VOLUME" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "يارد", Code = "YD", Description = "YARD (CM 91.4)" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سال", Code = "YY", Description = "YEAR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "دسی متر مکعب", Code = "DM", Description = "DESI M3" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "قاب", Code = "FR", Description = "FRAME" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "لوله", Code = "PP", Description = "PIPE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "واحد", Code = "UI", Description = "UNIQUE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "كيلو ريال", Code = "KRLS", Description = "KiloRials" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلومتر بر تن", Code = "KT", Description = "KiloMeterPerTon" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلومتر", Code = "KM", Description = "KILOMETER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "متر مکعب بر تن", Code = "MC", Description = "CUBIC METER / TON" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نرمال متر مکعب بر مگاوات ساعت", Code = "NMC", Description = "NORMAL M3 METER / MEGA WAT" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "اینچ قطر", Code = "IN", Description = "INCH / RADIUS" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "متر طول", Code = "MT", Description = "METER / LENGTH" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "اینچ قطر متر طول", Code = "IN", Description = "INCH / RADIUS / METER / LENGTH" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "خط", Code = "LINE", Description = "LINE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مجموعه", Code = "SET", Description = "SET" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مورد", Code = "INS", Description = "INSTANCE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "متر کازی", Code = "METRCASI", Description = "METRCASI" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مترآلمینیوم وایر", Code = "METRALWIRE", Description = "METRALWIRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مترکافی", Code = "METRCAFE", Description = "METRCAFE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "اصله", Code = "NMB", Description = "NUMBER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "رشته", Code = "STR", Description = "STRING" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تن - مایل دریایی", Code = "TMM", Description = "TON_MILE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "پل", Code = "BR", Description = "BRIDGE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مترمکعب - کیلومتر", Code = "MMK", Description = "MM_K" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "پانل", Code = "PN", Description = "PANEL" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "خم", Code = "CV", Description = "CURVE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کلمپ", Code = "C", Description = "CLAMP" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کاست", Code = "CS", Description = "KASET" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سلول", Code = "CEL", Description = "CELL" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "پره", Code = "WN", Description = "WNNG" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "دستگاه ساعت", Code = "DT", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "میلیمتر جیوه", Code = "MJ", Description = "Melimeter Mercury" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نرمال لیتر بر دقیقه", Code = "NL", Description = "NORMAL LITR PER MIN" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلو کالری بر ساعت", Code = "KCAL", Description = "KCAL/HOUR" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "صفحه", Code = "SHT", Description = "SHEET" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلو                                           تن", Code = "KTON", Description = "KiloTon" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "شب", Code = "NT", Description = "NIGHT" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ثانیه", Code = "SD", Description = "second" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "استاندارد متر مکعب", Code = "18", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تن بر ساعت", Code = "T", Description = "" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نرمال متر مکعب بر تن", Code = "NCM", Description = "NORMAL M3 METER / TON" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تن بر مگاوات ساعت", Code = "TON", Description = "TON / MEGA WAT" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلو کالری بر تن", Code = "KCAL", Description = "KILLO CALERY / TON" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سیم بکسل", Code = "WR", Description = "WIRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "زوج", Code = "CU", Description = "Couple" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "طبقه", Code = "STI", Description = "Stratum" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "تابلو", Code = "SIG", Description = "Tableau" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "لنگه", Code = "BA", Description = "Bale" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مترگرافیت وایر", Code = "MGRW", Description = "METRGRAFITWIRE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "لته", Code = "LTT", Description = "LAATE" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سانتیمتر مربع", Code = "CM2", Description = "CC2" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "اینچ مربع", Code = "IM", Description = "INCH M" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "اتاقک", Code = "ROOM", Description = "ROOM" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ست", Code = "SET01", Description = "SET" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "ست کامل", Code = "SET_ALL", Description = "SET ALL" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مقطوع", Code = "FIX", Description = "FIXED" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سیلندر", Code = "CYL", Description = "CYLINDER" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "متر مکعب", Code = "M3", Description = "METER3" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سیستم", Code = "SYS", Description = "SYSTEM" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیت", Code = "KIT", Description = "KIT" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "فوت مربع", Code = "FS", Description = "FOOT-SQU" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلوگرم بر دقیقه", Code = "KG", Description = "KiloGeramPerMin" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "مگا وار ساعت", Code = "MVH", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "بار", Code = "BAR", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "درصد", Code = "PCN", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "میکروزیمنس بر سانتیمتر", Code = "MS", Description = "" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "خوردگی بر سال", Code = "MPY", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "پرس", Code = "PS", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نفر شعاع", Code = "NS", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سانس", Code = "SA", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "جلسه", Code = "ME", Description = "session" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "نفر شب", Code = "NN", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "سرویس", Code = "SR", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "پالس", Code = "PA", Description = string.Empty });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "لیتر بر تن", Code = "L", Description = "LITR/TON" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "کیلوگرم بر تن", Code = "KG", Description = "KILOGERAM/TON" });
        //    list.Add(new Unit { Id = Guid.NewGuid(), IsActive = true, Name = "میلی کنر مکعب", Code = "MM3", Description = "CIOBIC MELIMETERS" });

        //    _dbContext.Units.AddRange(list);

        //    await _dbContext.SaveChangesAsync();
        //    return true;

        //}

    }

}
