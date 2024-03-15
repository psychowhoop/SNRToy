using System;
using UniFramework.Event;

public class UserEventDefine
{
	/// <summary>
	/// 用户尝试再次初始化资源包
	/// </summary>
	public class UserTryInitialize : IEventMessage
	{
		public Object pasData { get; set; }
		public static void SendEventMessage(Object machine)
		{
			var msg = new UserTryInitialize();
			msg.pasData = machine;
			UniEvent.SendMessage(msg);
		}
	}

	/// <summary>
	/// 用户开始下载网络文件
	/// </summary>
	public class UserBeginDownloadWebFiles : IEventMessage
	{
		public Object pasData { get; set; }
		public static void SendEventMessage(Object machine)
		{
			var msg = new UserBeginDownloadWebFiles();
			msg.pasData = machine;
			UniEvent.SendMessage(msg);
		}
	}

	/// <summary>
	/// 用户尝试再次更新静态版本
	/// </summary>
	public class UserTryUpdatePackageVersion : IEventMessage
	{
		public Object pasData { get; set; }
		public static void SendEventMessage(Object machine)
		{
			var msg = new UserTryUpdatePackageVersion();
			msg.pasData = machine;
			UniEvent.SendMessage(msg);
		}
	}

	/// <summary>
	/// 用户尝试再次更新补丁清单
	/// </summary>
	public class UserTryUpdatePatchManifest : IEventMessage
	{
		public Object pasData { get; set; }
		public static void SendEventMessage(Object machine)
		{
			var msg = new UserTryUpdatePatchManifest();
			msg.pasData = machine;
			UniEvent.SendMessage(msg);
		}
	}

	/// <summary>
	/// 用户尝试再次下载网络文件
	/// </summary>
	public class UserTryDownloadWebFiles : IEventMessage
	{
		public Object pasData { get; set; }
		public static void SendEventMessage(Object machine)
		{
			var msg = new UserTryDownloadWebFiles();
			msg.pasData = machine;
			UniEvent.SendMessage(msg);
		}
	}
}