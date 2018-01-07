using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedModule : ActorModule {

	public bool CanTarget(ITarget target) {
		return false;
		// TODO
	}

	public ITarget GetTarget() {
		return null;
	}

	public bool HasTarget(ITarget target) {
		return false;
	}

	public void SetTarget(ITarget target) {

	}
}
