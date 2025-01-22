using Domain.Models;

namespace MigrationTool.Extensions {
	public static class DomainExtensions {
		/// <summary>
		/// Place Pages in direct order
		/// </summary>
		/// <param name="bookPages"></param>
		/// <returns></returns>
		public static ICollection<Page> OrderByPrevPageId(this ICollection<Page> bookPages) {
			var pageOrder = new List<Page>();
			Page? first = bookPages?.FirstOrDefault(x => x.PreviousPageId == 0);
			if (first != null) {
				pageOrder.Add(first);
				int currentPrev = first.Id;
				Page? next = bookPages?.FirstOrDefault(x => x.PreviousPageId == currentPrev);
				while (next != null) {
					pageOrder.Add(next);
					currentPrev = next.Id;
					next = bookPages?.FirstOrDefault(x => x.PreviousPageId == currentPrev);
				}
			}
			return pageOrder;
		}
	}
}
