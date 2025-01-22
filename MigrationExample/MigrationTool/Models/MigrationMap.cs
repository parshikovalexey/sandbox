namespace MigrationTool.Models {
	public class MigrationMap {
		protected IDictionary<(Type sourceType, string Id), string> Map = new Dictionary<(Type type, string Id), string>();
		public void AddToMap<TType>(string sourceId, string targetId, TType obj) {
			Type type = typeof(TType);
			if (Map.ContainsKey((type, sourceId))) {
				Map[(type, sourceId)] = targetId;
			} else {
				Map.Add((type, sourceId), targetId);
			}
		}

		public bool TryGetTargetId(Type type, string sourceId, out string result) {
			if (Map.ContainsKey((type, sourceId))) {
				result = Map[(type, sourceId)];
				return true;
			}
			result = string.Empty;
			return false;
		}

		public bool TryGetTargetId<TType>(string sourceId, out int result) {
			result = 0;
			return TryGetTargetId(typeof(TType), sourceId, out string sid) && int.TryParse(sid, out result);
		}

		public bool TryGetTargetId<TType>(string sourceId, out Guid result) {
			result = Guid.Empty;
			return TryGetTargetId(typeof(TType), sourceId, out string sid) && Guid.TryParse(sid, out result);
		}

		public bool Contains<TType>(string sourceId) {
			return Map.ContainsKey((typeof(TType), sourceId));
		}
	}
}
