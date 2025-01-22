using DAL.Interfaces;
using Domain.Models;
using MigrationTool.Models;

namespace MigrationTool.Implementation.MigrationObjects {
	public class MigrationAuthor : MigrationObject<Author> {
		public MigrationAuthor(MigrationContext context, Author source) : base(context, source) {
		}

		public override async Task<Author> ConvertToTargetAsync(Author source, MigrationMap map) {
			return await MergeAsync(source, new Author() { Id = source.Id });
		}

		public override Task<Queue<MigrationObject>> FillAfterMigrationChildrenAsync(Author source, MigrationMap map) {
			return Task.FromResult(new Queue<MigrationObject>());
		}

		public override Task<Queue<MigrationObject>> FillBeforeMappingChildrenAsync(Author source, MigrationMap map) {
			return Task.FromResult(new Queue<MigrationObject>());
		}

		public override async Task<Author?> GetExistedObjectAsync(IDataService<Author> reader, Author source) {
			return await reader.FirstOrDefaultAsync(e => e.Id == source.Id);
		}

		public override string GetObjectId(Author source) {
			return source.Id.ToString();
		}

		public override Task<Author> MergeAsync(Author source, Author target) {
			target.Description = source.Description;
			target.Name = source.Name;
			return Task.FromResult(target);
		}
	}
}
