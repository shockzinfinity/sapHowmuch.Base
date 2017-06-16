using System;
using System.Collections;
using System.Reflection;

namespace sapHowmuch.Base.Helpers
{
	public class DynamicInvoke
	{
		private static Hashtable _assemblyReferences = new Hashtable();
		private static Hashtable _classReferences = new Hashtable();

		/// <summary>
		/// 로딩된 어셈블리 참조를 설정하거나 제공합니다.
		/// </summary>
		public static Hashtable AssemblyReferences
		{
			get { return _assemblyReferences; }
			set { _assemblyReferences = value; }
		}

		/// <summary>
		/// 로딩된 클래스 참조를 설정하거나 제공합니다.
		/// </summary>
		public static Hashtable ClassReferences
		{
			get { return _classReferences; }
			set { _classReferences = value; }
		}

		/// <summary>
		/// 로딩된 클래스 정보를 제공합니다.
		/// </summary>
		public class DynamicClassInfo
		{
			public Type type;
			public object ClassObject;

			/// <summary>
			/// 기본 생성자
			/// </summary>
			public DynamicClassInfo()
			{
			}

			public DynamicClassInfo(Type t, Object c)
			{
				type = t;
				ClassObject = c;
			}
		}

		/// <summary>
		/// 지정된 어셈블리의 메소드를 실행하는 기능을 제공합니다.
		/// </summary>
		/// <param name="AssemblyName">Assembly 전체경로 및 이름</param>
		/// <param name="ClassName">클래스 이름</param>
		/// <param name="MethodName">메소드 명</param>
		/// <param name="methodArgs">메소드 파라메터</param>
		/// <returns></returns>
		public static Object InvokeMethodSlow(string AssemblyName, string ClassName, string MethodName, Object[] methodArgs)
		{
			// load the assemly
			Assembly assembly = Assembly.LoadFrom(AssemblyName);

			// Walk through each type in the assembly looking for our class
			foreach (Type type in assembly.GetTypes())
			{
				if (type.IsClass == true)
				{
					if (type.FullName.EndsWith("." + ClassName))
					{
						// create an instance of the object
						object ClassObj = Activator.CreateInstance(type);

						// Dynamically Invoke the method
						object Result = type.InvokeMember(MethodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, ClassObj, methodArgs);
						return (Result);
					}
				}
			}
			throw (new System.Exception("could not invoke method"));
		}

		/// <summary>
		/// 지정된 어셈블리의 메소드를 실행하는 기능을 제공합니다.
		/// </summary>
		/// <param name="AssemblyName">Assembly 전체경로 및 이름</param>
		/// <param name="ClassName">클래스 이름</param>
		/// <param name="MethodName">메소드 명</param>
		/// <param name="classArgs">클래스 파라메터</param>
		/// <param name="methodArgs">메소드 파라메터</param>
		/// <returns></returns>
		public static Object InvokeMethodSlow(string AssemblyName, string ClassName, string MethodName, Object[] classArgs, Object[] methodArgs)
		{
			// load the assemly

			Assembly assembly = Assembly.LoadFrom(AssemblyName);

			// Walk through each type in the assembly looking for our class

			foreach (Type type in assembly.GetTypes())
			{
				if (type.IsClass == true)
				{
					if (type.FullName.EndsWith("." + ClassName))
					{
						// create an instance of the object

						//object ClassObj = Activator.CreateInstance(type);
						object ClassObj = Activator.CreateInstance(type, classArgs);

						// Dynamically Invoke the method

						object Result = type.InvokeMember(MethodName,
						  BindingFlags.Default | BindingFlags.InvokeMethod,
							   null,
							   ClassObj,
							   methodArgs);
						return (Result);
					}
				}
			}
			throw (new System.Exception("could not invoke method"));
		}

		/// <summary>
		/// 클래스 레퍼런스를 제공합니다.
		/// </summary>
		/// <param name="AssemblyName">어셈블리 전체경로</param>
		/// <param name="ClassName">클래스명</param>
		/// <returns></returns>
		public static DynamicClassInfo GetClassReference(string AssemblyName, string ClassName)
		{
			if (ClassReferences.ContainsKey(AssemblyName) == false)
			{
				Assembly assembly;
				if (AssemblyReferences.ContainsKey(AssemblyName) == false)
				{
					AssemblyReferences.Add(AssemblyName,
						  assembly = Assembly.LoadFrom(AssemblyName));
				}
				else
					assembly = (Assembly)AssemblyReferences[AssemblyName];

				// Walk through each type in the assembly
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsClass == true)
					{
						// doing it this way means that you don't have
						// to specify the full namespace and class (just the class)
						if (type.FullName.EndsWith("." + ClassName))
						{
							DynamicClassInfo ci = new DynamicClassInfo(type,
											   Activator.CreateInstance(type));
							ClassReferences.Add(AssemblyName, ci);
							return (ci);
						}
					}
				}
				throw (new System.Exception("could not instantiate class"));
			}
			return ((DynamicClassInfo)ClassReferences[AssemblyName]);
		}

		/// <summary>
		/// 메소드를 실행하는 기능을 제공합니다.
		/// </summary>
		/// <param name="ci">클래스 인스턴스</param>
		/// <param name="MethodName">메소드명</param>
		/// <param name="args">메소드 파라메터</param>
		/// <returns></returns>
		public static Object InvokeMethod(DynamicClassInfo ci, string methodName, Object[] args)
		{
			// Dynamically Invoke the method
			Object Result = ci.type.InvokeMember(methodName,
			  BindingFlags.Default | BindingFlags.InvokeMethod,
				   null,
				   ci.ClassObject,
				   args);
			return (Result);
		}

		/// <summary>
		/// 속성를 설정하는 기능을 제공합니다.
		/// </summary>
		/// <param name="ci">클래스 인스턴스</param>
		/// <param name="propertyName">속성 명</param>
		/// <param name="args">속성 파라메터</param>
		/// <returns></returns>
		public static Object InvokeProperty(DynamicClassInfo ci, string propertyName, Object[] args)
		{
			// Dynamically Invoke the property
			Object Result = ci.type.InvokeMember(propertyName,
			  BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public,
				   null,
				   ci.ClassObject,
				   args);
			return (Result);
		}

		/// <summary>
		/// 메소드를 실행하는 기능을 제공합니다.
		/// </summary>
		/// <param name="AssemblyName">어셈블리 전체경로</param>
		/// <param name="ClassName">클래스명</param>
		/// <param name="MethodName">메소드명</param>
		/// <param name="args">파라메터</param>
		/// <returns></returns>
		public static Object InvokeMethod(string AssemblyName, string ClassName, string MethodName, Object[] args)
		{
			DynamicClassInfo ci = GetClassReference(AssemblyName, ClassName);
			return (InvokeMethod(ci, MethodName, args));
		}
	}
}