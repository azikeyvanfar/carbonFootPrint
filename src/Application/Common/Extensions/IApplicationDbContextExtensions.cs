namespace ContractorBackend.Application.Common.Extensions;



//public class IApplicationDbContextExtensions
//{ 
//    public class EntityUsedException : Exception
//    {
//        public EntityUsedException(string message) : base(message) { }
//    }

//    public static async Task CheckEntityUsageAsync<TEntity>(IApplicationDbContext context, Guid entityId, EntityEntry entry) where TEntity : class
//    {
//        // Get the entity type
//        var entityType = ((DbContext)context).Model.FindEntityType(typeof(TEntity));

//        // Get navigation properties
//        var navigationProperties = entityType.GetNavigations().Where(x => x.IsCollection);

//        foreach (var navigation in navigationProperties)
//        {
//            // Get the related entity type
//            var relatedEntityType = navigation.GetTargetType();

//            // Create a parameter expression for the related entity
//            var parameter = Expression.Parameter(relatedEntityType.ClrType, "e");

//            // Create a property expression for the foreign key
//            var foreignKeyProperty = relatedEntityType.GetForeignKeys()
//                .Where(fk => fk.PrincipalEntityType == entityType)
//                .FirstOrDefault()?
//                .Properties
//                .FirstOrDefault();

//            if (foreignKeyProperty != null)
//            {
//                // Create a lambda expression to check if any record exists
//                var foreignKeyIdExpression = Expression.Property(parameter, foreignKeyProperty.Name);
//                var entityIdExpression = Expression.Constant(entityId);
//                var equalityExpression = Expression.Equal(foreignKeyIdExpression, entityIdExpression);

//                var lambda = Expression.Lambda(equalityExpression, parameter);
//                // Use reflection to create the DbSet for the related entity
//                var dbSetMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)
//                    .MakeGenericMethod(relatedEntityType.ClrType);
//                var dbSet = dbSetMethod.Invoke(context, null);

//                var anyMethod = typeof(Queryable).GetMethods()
//                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
//                    .MakeGenericMethod(relatedEntityType.ClrType);

//                // Check if any records exist referencing the entityId
//                var exists = (bool)anyMethod.Invoke(null, new object[] { dbSet, lambda });

//                if (exists)
//                {
//                    Log.Warning($"Entity with ID {entityId} is used by other records in navigation '{navigation.Name}'.");
//                    entry.State = EntityState.Unchanged;

//                    throw new DbUpdateException("aa");
//                    //throw new CustomException("سطر مورد نظر داای رکورد فعال است.");

//                }
//            }
//        }
//    }



//}