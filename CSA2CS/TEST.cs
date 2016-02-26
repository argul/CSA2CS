using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CSA2CS.TEST
{
	public enum TEST_ENUM
	{
		ERROR = -1,
		ZERO = 0,
		ONE = 1,
		TWO = 2,
		THREE = 3
	}

	internal interface TEST_INTERFACE
	{
		void Foo();
		void Bar(out string str);
	}

	public struct TEST_STRUCT
	{
		public int a;
		public void Foo() {}
		public NESTED nested;

		public struct NESTED
		{
			public int a;
		}
	}

	public class TEST_CONSTS
	{
		public const int a = 0;
		protected const int b = 100;
		private const int c = 1000;
	}

	public class TEST_CONSTRUCTOR
	{
		public TEST_CONSTRUCTOR() {}
		protected TEST_CONSTRUCTOR(int a) {}
		private TEST_CONSTRUCTOR(int a, int b) {}
	}

	public class TEST_FINALIZER
	{
		~TEST_FINALIZER() {}
	}

	public delegate void GlobalDelegate();
	public class TEST_DELEGATE
	{
		public delegate void NestedDelegate();

		public GlobalDelegate globalDelegate;
		public NestedDelegate nestedDelegate;
	}

	public delegate void GlobalEvent();
	public class TEST_EVENT
	{
		public TEST_EVENT() { globalEvent += ()=>{}; nestedEvent += ()=>{}; }
		public delegate void NestedEvent();

		public event GlobalEvent globalEvent;
		public event NestedEvent nestedEvent;
	}

	public class TEST_FIELD
	{
		public int a;
		public static int b;
	}

	public class TEST_PROPERTY
	{
		public static int A 
		{ 
			get { return 0; }
			private set { }
		}

		public int B
		{
			get { return 0; }
		}

		public int C
		{
			set { }
		}
	}

	public class TEST_METHOD
	{
		public void Foo() {}
		public TEST_METHOD Bar(int a, 
		                       System.Reflection.MethodInfo b, 
		                       TEST_METHOD c, 
		                       System.Collections.Generic.Dictionary<string, TEST_METHOD> d) { return null; }
	}

	public class TEST_ANONYMOUS
	{
		public void Foo()
		{
			System.Action<int> anonymous = (a)=>{};
			anonymous.Invoke(0);
		}
	}

	public class TEST_COROUTINE
	{
		public System.Collections.IEnumerator Coroutine()
		{
			yield break;
		}
	}

	public abstract class TEST_ABSTRACT
	{
		public abstract TEST_ABSTRACT Foo(TEST_ABSTRACT a);
	}

	public class TEST_PRIVACY
	{
		public TEST_PRIVACY() { A(e); }
		public int a;
		protected int b;
		protected internal int c;
		internal int d;
		private int e = 0;

		public void A(int a) {}
		protected void B() {}
		protected internal void C() {}
		internal void D() {}
		private void E() {}
	}

	public class TEST_TRAIT
	{
		public interface A
		{
			void Foo();
			void Bar();
		}
		public interface D
		{
			void None();
		}
		public class B : A
		{
			public void Foo() {}
			public virtual void Bar() {}
			public virtual void Zak() {}
			public virtual void Yeh() {}
		}
		public class C : B, D
		{
			public override void Bar () {}
			public sealed override void Zak () {}
			public virtual void Gps() {}
			public void None() {}
		}
	}

	public class TEST_GENERIC<K, T>
	{
		public K field;
		public K Method(T a) { return default(K); }
		public K Property { get { return default(K); } set { } }
		public K Method_Generic<V, M, N>(K k, T t) { return default(K); }
	}

	public class TEST_GENERIC2<S, K> : TEST_GENERIC<K, string>
	{
		public S field;
		public Dictionary<S, K> Method2(string a) { return null; }
		public K Property2 { get { return default(K); } set { } }
	}

	public struct TEST_OPERATOR
	{
//		public static implicit operator TEST_OPERATOR(bool value)
//		{
//			return false;
//		}

		public static explicit operator TEST_OPERATOR(bool value)
		{
			return default(TEST_OPERATOR);
		}

		public static bool operator < (TEST_OPERATOR lhs, TEST_OPERATOR rhs)
		{
			return false;
		}
		
		public static bool operator > (TEST_OPERATOR lhs, TEST_OPERATOR rhs){
			return false;
		}
		
		public static bool operator == (TEST_OPERATOR lhs, TEST_OPERATOR rhs){
			return true;
		}
		
		public static bool operator != (TEST_OPERATOR lhs, TEST_OPERATOR rhs){
			return false;
		}

		public int this[int x] { get { return 0; } }
		public int this[int x, TEST_TRAIT y, TEST_GENERIC2<int, int> z] { get { return 0; } }
		
		public override string ToString ()
		{
			return String.Empty;
		}
		
		public override bool Equals (object other)
		{
			if (!(other is TEST_OPERATOR)){
				return false;
			}
			return true;
		}
		
		public override int GetHashCode ()
		{
			return 0;
		}
	}

	public static class TEST_THIS
	{
		public class TEST
		{
		}
		public static void TEST_THIS_METHOD(this TEST t, int a) { }
	}

	public static class TEST_HIDE
	{
		public class BASE
		{
			public void Foo() {}
		}

		public class DERIVED : BASE
		{
			public new void Foo() {}
		}
	}

	public static class TEST_NATIVE_INVOKE
	{
		[DllImport("test.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
		internal static extern int TEST(IntPtr hWnd, ref String text, String caption, uint type = 0);
	}
}

