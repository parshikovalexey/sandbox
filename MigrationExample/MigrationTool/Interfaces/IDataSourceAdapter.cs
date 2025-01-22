using DAL.Interfaces;

namespace MigrationTool.Interfaces {
	public interface IDataSourceAdapter {
		IDataService<TDomain> GetSource<TDomain>() where TDomain : class;
		IDataService<TDomain> GetTarget<TDomain>() where TDomain : class;
	}
}
