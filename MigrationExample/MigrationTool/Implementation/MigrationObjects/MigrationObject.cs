using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using MigrationTool.Models;

namespace MigrationTool.Implementation.MigrationObjects
{
    public abstract class MigrationObject {
		public abstract Task<bool> MigrateAsync(MigrationMap map);
	}

	public abstract class MigrationObject<TDomain> : MigrationObject where TDomain : class {
		protected readonly ILogger Logger;
		protected readonly IDataService<TDomain> SourceDataService;
		protected readonly IDataService<TDomain> TargetDataService;
		protected readonly MigrationContext Context;

		public TDomain Source { get; private set; }

		public MigrationObject(MigrationContext context, TDomain source) {
			Logger = context.Logger;
			Source = source;
			Context = context;
			SourceDataService = context.SourceDataAdapter.GetSource<TDomain>();
			TargetDataService = context.SourceDataAdapter.GetTarget<TDomain>();
		}

		/// <summary>
		/// Migrate Source object to the Target storage
		/// </summary>
		/// <param name="map">Migration map</param>
		/// <returns></returns>
		public override async Task<bool> MigrateAsync(MigrationMap map) {
			try {
				// Migrate objects before main Migration to fill the map
				Queue<MigrationObject> beforeChildren = await FillBeforeMappingChildrenAsync(Source, map);
				await MigrateChildrenAsync(beforeChildren, map);

				// Prepare target object
				var target = await ConvertToTargetAsync(Source, map);
				if (target == null) {
					Logger.LogError("No target object to migrate");
					return false;
				}

				// Check existense 
				TDomain? targetObj = await GetExistedObjectAsync(TargetDataService, target);
				if (targetObj == null) {
					// Try add object in the Target service
					targetObj = await TargetDataService.AddAsync(target);
					if (targetObj == null) {
						Logger.LogError($"Could not add {typeof(TDomain).Name} to the Target context");
						return false;
					}
				} else if (AllowUpdateOnExist()) {
					targetObj = await MergeAsync(target, targetObj);
					targetObj = await TargetDataService.UpdateAsync(targetObj);
					if (targetObj == null) {
						Logger.LogError($"Could not update {typeof(TDomain).Name} in the Target context");
						return false;
					}
				}

				// Add new object in the migration map
				map.AddToMap(GetObjectId(Source), GetObjectId(targetObj), targetObj);

				// Migrate last children
				Queue<MigrationObject> afterChildren = await FillAfterMigrationChildrenAsync(Source, map);
				return await MigrateChildrenAsync(afterChildren, map);
			} catch (Exception ex) {
				Logger.LogError(ex, $"Could not migrate {typeof(TDomain)}");
				return false;
			}
		}

		/// <summary>
		/// Make migration for children migration objecst
		/// </summary>
		/// <param name="children">Migrations objects to be migrated</param>
		/// <param name="map">Migration map</param>
		/// <param name="afterMigrated">Action to be called after succesfull migration</param>
		/// <returns></returns>
		protected async Task<bool> MigrateChildrenAsync(Queue<MigrationObject> children, MigrationMap map, Action<MigrationObject>? afterMigrated = null) {
			while (children.Count > 0) {
				MigrationObject child = children.Dequeue();
				bool result = await child.MigrateAsync(map);
				if (!result) {
					Logger.LogError("The child is not migrated");
					return false;
				}
				afterMigrated?.Invoke(child);
			}
			return true;
		}

		/// <summary>
		/// Fill migration queue for objects, which should be migrated before the entire object
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="map">Migration map</param>
		/// <returns>Queue of objects to migrate before the entire object</returns>
		public abstract Task<Queue<MigrationObject>> FillBeforeMappingChildrenAsync(TDomain source, MigrationMap map);
		/// <summary>
		/// Fill migration queue for objects, which should be migrated after the entire object
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="map">Migration map</param>
		/// <returns>Queue of objects to migrate after the entire object</returns>
		public abstract Task<Queue<MigrationObject>> FillAfterMigrationChildrenAsync(TDomain source, MigrationMap map);
		/// <summary>
		/// Prepare object to be saved to Target storage
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="map">Migration map</param>
		/// <returns>Object for Target storage</returns>
		public abstract Task<TDomain> ConvertToTargetAsync(TDomain source, MigrationMap map);
		/// <summary>
		/// Search existed objects in Target storage
		/// </summary>
		/// <param name="reader">Taget storage reader service</param>
		/// <param name="source">Source object</param>
		/// <returns></returns>
		public abstract Task<TDomain?> GetExistedObjectAsync(IDataService<TDomain> reader, TDomain source);
		/// <summary>
		/// Get string presentation of the object Id
		/// </summary>
		/// <param name="source">Source object</param>
		/// <returns>String presentation of the objects Id</returns>
		public abstract string GetObjectId(TDomain source);
		/// <summary>
		/// Merge target object with source and return it
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="target">Target object</param>
		/// <returns>Object to target storage</returns>
		public abstract Task<TDomain> MergeAsync(TDomain source, TDomain target);
		/// <summary>
		/// Indicates the migration objects could be updated if exists in the Target storage
		/// </summary>
		/// <returns>True if object could be updated, otherwise - false</returns>
		public virtual bool AllowUpdateOnExist() => false;
	}

}
