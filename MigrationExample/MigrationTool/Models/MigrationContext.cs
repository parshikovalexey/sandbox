using Microsoft.Extensions.Logging;
using MigrationTool.Interfaces;

namespace MigrationTool.Models {
	public class MigrationContext {
		public readonly ILogger Logger;
		public readonly IDataSourceAdapter SourceDataAdapter;

		public MigrationContext(ILogger logger, IDataSourceAdapter sourceAdapter) { 
			SourceDataAdapter = sourceAdapter;
			Logger = logger;
		}
	}
}
