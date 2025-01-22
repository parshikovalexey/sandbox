namespace Domain.Models {
	public class Book {
		public Book() {
			Pages = new HashSet<Page>();
		}
		
		public Guid Id { get; set; }
		public Guid? AuthorId { get; set; }
		public string Title { get; set; } = string.Empty;
		public DateTime? CreatedAt { get; set; }
		public DateTime? LastUpdatedAt { get; set; }

		public Author? Author { get; set; }
		public virtual ICollection<Page> Pages { get; set; }
	}
}
