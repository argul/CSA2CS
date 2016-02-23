namespace CSA2CS
{
	public interface IPoolUser
	{
		string Token { get; }
		void OnSpawn();
		void OnRecycle();
	}
}
