﻿using System;

namespace GBaaS.io {

	public enum ReturnCode : int {
		Success = 0,
		SuccessWithReasonAsResult = 1,
		SuccessButAlt = 2,
		BeforeTry = -1,
		WaitAsync = -2,
		FailWithReason = -3,
		CheckPreCondition = -4,
		Exception = -900
	}

	public class GBResult {
		public GBResult () {
			isSuccess 	= false;
			returnCode 	= ReturnCode.BeforeTry;
			reason 		= "Before Try";
		}		
			
		public void MakeResult(bool isSuccess, ReturnCode returnCode, string reason) {
			this.isSuccess = isSuccess;
			this.returnCode = returnCode;
			this.reason = reason;
		}

		public bool 		isSuccess;
		public ReturnCode	returnCode;
		public string 		reason;
	}
}
