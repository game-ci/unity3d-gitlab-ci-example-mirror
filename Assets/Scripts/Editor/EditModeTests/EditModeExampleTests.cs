using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class EditModeExampleTests
{

	[Test]
	public void EditModeExampleTestSimplePasses ()
	{
		// Use the Assert class to test conditions.
		Assert.True(true);
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator EditModeExampleTestWithEnumeratorPasses ()
	{
		// Use the Assert class to test conditions.
		// yield to skip a frame
		Assert.True(true);
		yield return null;
	}
}
