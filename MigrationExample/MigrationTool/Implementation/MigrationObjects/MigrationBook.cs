using DAL.Interfaces;
using Domain.Models;
using MigrationTool.Models;
using MigrationTool.Extensions;

namespace MigrationTool.Implementation.MigrationObjects {
	public class MigrationBook : MigrationObject<Book> {
		public MigrationBook(MigrationContext context, Book source) : base(context, source) {
		}

		public override async Task<Book> ConvertToTargetAsync(Book source, MigrationMap map) {
			Book book = await MergeAsync(source, new Book());
			if (source.AuthorId.HasValue
				&& map.TryGetTargetId<Author>(source.AuthorId.Value.ToString(), out Guid authorId)
			) {
				book.AuthorId = authorId;
			}

			return book;
		}

		public override Task<Queue<MigrationObject>> FillAfterMigrationChildrenAsync(Book source, MigrationMap map) {
			var queue = new Queue<MigrationObject>();
			ICollection<Page> orderedPages = source.Pages.OrderByPrevPageId();
			foreach (Page page in orderedPages) {
				var migrationPage = new MigrationPage(Context, page);
				queue.Enqueue(migrationPage);
			}
			return Task.FromResult(queue);
		}

		public override Task<Queue<MigrationObject>> FillBeforeMappingChildrenAsync(Book source, MigrationMap map) {
			var queue = new Queue<MigrationObject>();
			if (source.Author != null && !map.Contains<Author>(source.Author.Id.ToString())) {
				queue.Enqueue(new MigrationAuthor(Context, source.Author));
			} 
			return Task.FromResult(queue);
		}

		public override async Task<Book?> GetExistedObjectAsync(IDataService<Book> reader, Book source) {
			return await reader.FirstOrDefaultAsync(e => e.Id == source.Id);
		}

		public override string GetObjectId(Book source) {
			return source.Id.ToString();
		}

		public override Task<Book> MergeAsync(Book source, Book target) {
			target.CreatedAt = source.CreatedAt;
			target.LastUpdatedAt = source.LastUpdatedAt;
			target.Title = source.Title;
			return Task.FromResult(target);
		}
	}
}
