using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple type that can be returned from a method to indicate 
/// whether or not that method call succeeded, with an optional message
/// to include as well.
/// </summary>
public class OperationResult {
    /// <summary>The different result statuses that are possible.</summary>
	public enum Status { OK, PARTIAL, FAIL, CANCEL };

    /// <summary>The result status.</summary>
    public Status status;
    /// <summary>An optional message to include with the result status.</summary>
	public string message;

    /// <summary>
    /// Create an OperationResult with no message.
    /// </summary>
    /// <param name="status">The status of the operation.</param>
	public OperationResult(OperationResult.Status status) {
		this.status = status;
	}

    /// <summary>
    /// Create an OperationResult with a message.
    /// </summary>
    /// <param name="status">The status of the operation.</param>
    /// <param name="message">The optional message to describe the result.</param>
	public OperationResult(OperationResult.Status status, string message) {
		this.status = status;
		this.message = message;
	}

	/// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator ==(OperationResult result, OperationResult.Status status) {
		return result.status == status;
	}
	/// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator !=(OperationResult result, OperationResult.Status status) {
		return result.status != status;
	}

	/// <summary>Shortcut method for creating a new OK <c>OperationResult</c>.</summary>
	public static OperationResult OK(string message=null) {
		return new OperationResult(OperationResult.Status.OK, message);
	}

	/// <summary>Shortcut method for creating a new PARTIAL <c>OperationResult</c>.</summary>
	public static OperationResult Partial(string message=null) {
		return new OperationResult(OperationResult.Status.PARTIAL, message);
	}

	/// <summary>Shortcut method for creating a new FAIL <c>OperationResult</c>.</summary>
	public static OperationResult Fail(string message=null) {
		return new OperationResult(OperationResult.Status.FAIL, message);
	}

	/// <summary>Shortcut method for creating a new CANCEL <c>OperationResult</c>.</summary>
	public static OperationResult Cancel(string message=null) {
		return new OperationResult(OperationResult.Status.CANCEL, message);
	}
}
