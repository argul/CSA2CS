using System;
namespace CSA2CS
{
	public static class Assert
	{
		public static void AssertIsTrue(bool b, string error = null)
		{
			if (!b)
			{
				Error(error);
			}
		}
		
		public static void AssertNotNull(object obj, string error = null)
		{
			AssertIsTrue(!object.ReferenceEquals(null, obj), error);
		}

		public static void Error(string error)
		{
			if (String.IsNullOrEmpty(error))
			{
				throw new Exception("Assert failure");
			}
			else
			{
				throw new Exception("Assert failure : " + error);
			}
		}
	}
}

