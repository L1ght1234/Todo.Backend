using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo_Backend.DAL.Entities;

namespace Todo_Backend.DAL.Data.EntityTypeConfigurations;
public class TodoConfig : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Title)
            .IsRequired();

        builder.Property(e => e.Deadline);

        builder.Property(e => e.Status)
            .IsRequired();
    }
}