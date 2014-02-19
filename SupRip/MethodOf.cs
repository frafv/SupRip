using System;
using System.Reflection;

namespace SupRip
{
	public class MethodOf<T>
	{
		public MethodOf(T func)
		{
			var del = func as Delegate;
			if (del == null) throw new ArgumentException("Cannot convert func to Delegate.", "func");

			Method = del.Method;
		}

		private MethodInfo Method { get; set; }

		public static implicit operator MethodOf<T>(T func)
		{
			return new MethodOf<T>(func);
		}

		public static implicit operator MethodInfo(MethodOf<T> methodOf)
		{
			return methodOf.Method;
		}
	}
}
