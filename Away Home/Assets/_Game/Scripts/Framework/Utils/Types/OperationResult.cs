using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationResult {
	public enum Status { OK, PARTIAL, FAIL, CANCEL };

	public string message;
	public Status status;

	public OperationResult(OperationResult.Status status) {
		this.status = status;
	}

	public OperationResult(OperationResult.Status status, string message) {
		this.status = status;
		this.message = message;
	}

	public readonly static OperationResult OK = new OperationResult(Status.OK);
}
