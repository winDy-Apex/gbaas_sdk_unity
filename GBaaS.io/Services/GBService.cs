using System;
using GBaaS.io.Utils;

namespace GBaaS.io.Services
{
	public class GBService<DerivedType> : Singleton<DerivedType> where DerivedType : new()
	{
		public GBService () {}
	}
}
