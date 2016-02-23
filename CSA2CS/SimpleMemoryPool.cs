using System;
using System.Collections.Generic;

namespace CSA2CS
{
	public class SimpleMemoryPool
	{
		public void RegisterCreator(Func<string, IPoolUser> creator)
		{
			this.creator = creator;
		}

		public void Spawn(string token)
		{
			DoSpawn(token);
		}

		public void Recycle(IPoolUser obj)
		{
			DoRecycle(obj);
		}

		protected Dictionary<string, Stack<IPoolUser>> pool = new Dictionary<string, Stack<IPoolUser>>();
		protected Func<string, IPoolUser> creator;
		protected IPoolUser DoSpawn(string token)
		{
			Stack<IPoolUser> stack = null;
			pool.TryGetValue(token, out stack);
			if (null != stack && stack.Count > 0)
			{
				return stack.Pop();
			}
			else
			{
				return DoCreate(token);
			}
		}

		protected void DoRecycle(IPoolUser obj)
		{
			var token = obj.Token;
			Stack<IPoolUser> stack = null;
			if (!pool.TryGetValue(token, out stack))
			{
				stack = new Stack<IPoolUser>();
				pool.Add(token, stack);
			}
			stack.Push(obj);
		}

		protected IPoolUser DoCreate(string token)
		{
			return creator.Invoke(token);
		}
	}
}