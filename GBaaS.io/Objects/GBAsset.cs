using System;
using GBaaS.io.Services;
using GBaaS.io.Utils;

namespace GBaaS.io.Objects
{
	public class GBAsset : GBObject
	{
		public GBAsset() {
			this.SetEntityType("asset");
		}

		public string owner { get; set; }
		public string path { get; set; }
		public string name { get; set; }
	}
}