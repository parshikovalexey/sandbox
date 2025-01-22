namespace Domain.Models {
	public class Author {
		public Author() {
			Books = new HashSet<Book>();
		}

		public Guid Id { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public virtual ICollection<Book> Books { get; set; }
	}
}
