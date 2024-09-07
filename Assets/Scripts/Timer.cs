using UnityEngine;
using System.Collections;

public static class Timer
{
	private static MonoBehaviour _behaviour;

	public delegate void Task();

	public static void Schedule(MonoBehaviour behaviour, float delay, Task task)
	{
		_behaviour = behaviour;
		_behaviour.StartCoroutine(DoTask(task, delay));
	}

	private static IEnumerator DoTask(Task task, float delay)
	{
		yield return new WaitForSeconds(delay);
		task();
	}
}