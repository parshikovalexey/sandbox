using DAL.Interfaces;
using Domain.Models;
using MigrationTool.Models;

namespace MigrationTool.Implementation.MigrationObjects {
	public class MigrationPage : MigrationObject<Page> {
		public MigrationPage(MigrationContext context, Page source) : base(context, source) {
		}

		public override async Task<Page> ConvertToTargetAsync(Page source, MigrationMap map) {
			Page page = await MergeAsync(source, new Page { });
			int previousPageId = 0;
			if (source.PreviousPageId > 0
				&& map.TryGetTargetId<Page>(source.PreviousPageId.ToString(), out int id)
			) {
				previousPageId = id;
			}
			page.PreviousPageId = previousPageId;
			return page;
		}

		public override Task<Queue<MigrationObject>> FillAfterMigrationChildrenAsync(Page source, MigrationMap map) {
			return Task.FromResult(new Queue<MigrationObject>());
		}

		public override Task<Queue<MigrationObject>> FillBeforeMappingChildrenAsync(Page source, MigrationMap map) {
			return Task.FromResult(new Queue<MigrationObject>());
		}

		public override async Task<Page?> GetExistedObjectAsync(IDataService<Page> reader, Page source) {
			return await reader.FirstOrDefaultAsync(p =>
				p.BookId == source.BookId
				&& p.PreviousPageId == source.PreviousPageId
			);
		}

		public override string GetObjectId(Page source) {
			return source.Id.ToString();
		}

		public override Task<Page> MergeAsync(Page source, Page target) {
			target.CreatedAt = source.CreatedAt;
			target.LastUpdatedAt = source.LastUpdatedAt;
			target.Content = source.Content;
			return Task.FromResult(target);
		}
	}
}
