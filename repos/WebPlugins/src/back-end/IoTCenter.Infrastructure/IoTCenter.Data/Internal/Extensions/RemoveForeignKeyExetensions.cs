// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IoTCenter.Data.Internal
{
    public static class RemoveForeignKeyExetension
    {
        public static ModelBuilder RemoveForeignKeys(this ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes().ToList();

            entityTypes.ForEach(e =>
            {
                e.GetDeclaredReferencingForeignKeys().ToList().ForEach(f => 
                {
                    f.DeclaringEntityType?.RemoveForeignKey(f);
                });
            });

            return modelBuilder;
        }
    }
}
