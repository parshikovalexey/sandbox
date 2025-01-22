namespace Domain.Models {
	public class Page {
		public int Id { get; set; }
		public Guid BookId { get; set; }
		public string? Content { get; set; } = string.Empty;
		public DateTime? CreatedAt { get; set; }
		public DateTime? LastUpdatedAt { get; set; }
		public int PreviousPageId { get; set; }

		public Book? Book { get; set; }
	}
}
