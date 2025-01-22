using System.Linq.Expressions;

namespace DAL.Interfaces {
	public interface IDataService<TDomain> where TDomain : class {
		Task<TDomain?> AddAsync(TDomain t);
		Task<bool> DeleteAsync(TDomain t);
		Task<TDomain?> UpdateAsync(TDomain t);
		Task<TDomain?> FirstOrDefaultAsync(Expression<Func<TDomain, bool>> predicate);
	}
}
