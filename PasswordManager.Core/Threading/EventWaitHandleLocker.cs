using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PasswordManager.Core.Threading
{
	/// <summary>
	/// A <see cref="WaitHandle"/> based multi-threaded code execution control class.
	/// Compatible with the <c>using</c> statement and implements the <see cref="IDisposable"/> interface.
	/// </summary>
	public class EventWaitHandleLocker : IDisposable
	{
		#region Properties

		private readonly EventWaitHandle _waitHandle;
		/// <summary>
		/// An event wait handle being awaited.
		/// </summary>
		public EventWaitHandle WaitHandle { get { return _waitHandle; } }

		private readonly bool _waitHasSucceeded;
		/// <summary>
		/// Indicates whether the wait has succeeded (<c>true</c>) or not (<c>false</c>).
		/// </summary>
		public bool WaitHasSucceeded { get { return _waitHasSucceeded; } }

		private readonly bool _autoResetHandle;
		/// <summary>
		/// Indicates whether the event wait handle is auto-reset handle.
		/// </summary>
		public bool AutoResetHandle { get { return _autoResetHandle; } }

		private readonly bool _shouldSignalHandleOnDispose;
		/// <summary>
		/// Indicates if the locker should signal related handle on the Dispose() call.
		/// </summary>
		public bool ShouldSignalHandleOnDispose { get { return _shouldSignalHandleOnDispose; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new insance of the <see cref="EventWaitHandleLocker"/> class using the specified wait handle instance and waits for the handle to be signalled.
		/// </summary>
		/// <param name="waitHandle">An <see cref="EventWaitHandle"/> class instance to be used by the locker.</param>
		/// <param name="autoResetHandle">Specifies if the specified event handle is auto-reset handle.</param>
		/// <param name="shouldSignalHandleOnDispose">Specifies if the locker should signal the specified event handle being disposed. Default value is <c>true</c>.</param>
		public EventWaitHandleLocker(EventWaitHandle waitHandle, bool autoResetHandle, bool shouldSignalHandleOnDispose = true)
		{
			if (waitHandle == null)
				throw new ArgumentNullException("waitHandle");

			this._waitHandle = waitHandle;
			this._autoResetHandle = autoResetHandle;
			this._shouldSignalHandleOnDispose = shouldSignalHandleOnDispose;

			this._waitHasSucceeded = this.WaitHandle.WaitOne();
		}

		/// <summary>
		/// Creates a new insance of the <see cref="EventWaitHandleLocker"/> class using the specified wait handle instance and waits for the handle to be signalled with the specified timeout.
		/// </summary>
		/// <param name="waitHandle">An <see cref="EventWaitHandle"/> class instance to be used by the locker.</param>
		/// <param name="waitTimeout">A wait handle wait timeout time span.</param>
		/// <param name="autoResetHandle">Specifies if the specified event handle is auto-reset handle.</param>
		/// <param name="shouldSignalHandleOnDispose">Specifies if the locker should signal the specified event handle being disposed. Default value is <c>true</c>.</param>
		public EventWaitHandleLocker(EventWaitHandle waitHandle, TimeSpan waitTimeout, bool autoResetHandle, bool shouldSignalHandleOnDispose = true)
		{
			if (waitHandle == null)
				throw new ArgumentNullException("waitHandle");

			_waitHandle = waitHandle;
			this._autoResetHandle = autoResetHandle;
			this._shouldSignalHandleOnDispose = shouldSignalHandleOnDispose;

			this._waitHasSucceeded = this.WaitHandle.WaitOne(waitTimeout);
		}

		#endregion

		#region Factory methods

		/// <summary>
		/// Initializes a new instance of the <see cref="EventWaitHandleLocker"/> class using a
		/// co-initialized instance of the <see cref="EventWaitHandle"/> class used as the
		/// wait handle, specifying whether the wait handle is initially signaled if created
		/// as a result of this call, whether it resets automatically or manually, the name
		/// of a system synchronization event, and a Boolean variable whose value after the
		/// call indicates whether the named system event was created.
		/// </summary>
		/// <param name="initialState">Specify <c>true</c> to set the initial state to signaled
		/// if the named event is created as a result of this call; <c>false</c> to set it to
		/// nonsignaled.</param>
		/// <param name="mode">One of the <see cref="EventResetMode"/> values
		/// that determines whether the event resets automatically or manually.</param>
		/// <param name="name">The name of a system-wide synchronization event.</param>
		/// <returns></returns>
		public static EventWaitHandleLocker MakeWithEventHandle(bool initialState,
			EventResetMode mode,
			string name)
		{
			var eventWaitHandle = new EventWaitHandle(initialState, mode, name);
			var instance = new EventWaitHandleLocker(eventWaitHandle, mode == EventResetMode.AutoReset);
			return instance;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventWaitHandleLocker"/> class using a
		/// co-initialized instance of the <see cref="EventWaitHandle"/> class used as the
		/// wait handle, specifying whether the wait handle is initially signaled if created
		/// as a result of this call, whether it resets automatically or manually, the name
		/// of a system synchronization event, and a Boolean variable whose value after the
		/// call indicates whether the named system event was created.
		/// </summary>
		/// <param name="initialState">Specify <c>true</c> to set the initial state to signaled
		/// if the named event is created as a result of this call; <c>false</c> to set it to
		/// nonsignaled.</param>
		/// <param name="mode">One of the <see cref="EventResetMode"/> values
		/// that determines whether the event resets automatically or manually.</param>
		/// <param name="name">The name of a system-wide synchronization event.</param>
		/// <param name="createdNew">When this method returns, contains true if a local event
		/// was created (that is, if name is null or an empty string) or if the specified named
		/// system event was created; false if the specified named system event already existed.
		/// This parameter is passed uninitialized.</param>
		/// <returns></returns>
		public static EventWaitHandleLocker MakeWithEventHandle(bool initialState,
			EventResetMode mode,
			string name,
			out bool createdNew)
		{
			var eventWaitHandle = new EventWaitHandle(initialState, mode, name, out createdNew);
			var instance = new EventWaitHandleLocker(eventWaitHandle, mode == EventResetMode.AutoReset);
			return instance;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventWaitHandleLocker"/> class using a
		/// co-initialized instance of the <see cref="EventWaitHandle"/> class used as the
		/// wait handle, specifying whether the wait handle is initially signaled if created
		/// as a result of this call, whether it resets automatically or manually, the name
		/// of a system synchronization event, and a Boolean variable whose value after the
		/// call indicates whether the named system event was created.
		/// </summary>
		/// <param name="initialState">Specify <c>true</c> to set the initial state to signaled
		/// if the named event is created as a result of this call; <c>false</c> to set it to
		/// nonsignaled.</param>
		/// <param name="mode">One of the <see cref="EventResetMode"/> values
		/// that determines whether the event resets automatically or manually.</param>
		/// <param name="name">The name of a system-wide synchronization event.</param>
		/// <param name="createdNew">When this method returns, contains true if a local event
		/// was created (that is, if name is null or an empty string) or if the specified named
		/// system event was created; false if the specified named system event already existed.
		/// This parameter is passed uninitialized.</param>
		/// <returns></returns>
		public static EventWaitHandleLocker MakeWithEventHandle(bool initialState,
			EventResetMode mode,
			string name,
			out bool createdNew,
			TimeSpan timeout)
		{
			if (timeout < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("timeout",
					timeout,
					"Timeout is a negative number other than -1 milliseconds, which represents an infinite time-out.-or-timeout is greater than System.Int32.MaxValue.");
			}

			var eventWaitHandle = new EventWaitHandle(initialState, mode, name, out createdNew);
			var instance = new EventWaitHandleLocker(eventWaitHandle, timeout, mode == EventResetMode.AutoReset);
			return instance;
		}

		#endregion

		#region IDisposable implementation

		public bool IsDisposed { get; private set; }

		~EventWaitHandleLocker()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (IsDisposed)
				return;

			if (disposing)
			{
				this.WaitHandle.Set();

				//Free managed resources
				this.WaitHandle.Dispose();
			}

			//Free unmanaged resources

			IsDisposed = true;
		}

		#endregion
	}
}
