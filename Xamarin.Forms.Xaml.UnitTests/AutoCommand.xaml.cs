using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.Xaml.UnitTests
{
	partial class AutoCommand : ContentView
	{
		public AutoCommand()
		{
			InitializeComponent();
		}

		public AutoCommand(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		class Tests : Core.UnitTests.BaseTestFixture
		{
			[TestCaseSource(nameof(Command_Execution_Test_Data))]
			public void Command_Execution(bool useCompiledXaml
				, bool enableButton1
				, bool enableButton2
				, IEnumerable<string> expectedCallsStack)
			{
				var vm = new MockViewModel(0);

				var layout = new AutoCommand(useCompiledXaml)
				{
					BindingContext = vm,
				};

				vm.EnableButton1 = enableButton1;
				vm.EnableButton2 = enableButton2;

				(layout.Button1 as IButtonController)
					?.SendClicked();
				(layout.Button2 as IButtonController)
					?.SendClicked();
				(layout.ButtonDoAll as IButtonController)
					?.SendClicked();
				(layout.ButtonInvalid as IButtonController)
					?.SendClicked();

				CollectionAssert.AreEqual(expectedCallsStack
					, vm.CallsStack);
			}

			[TestCase(false)]
			[TestCase(true)]
			public void BindingContext_Changing(bool useCompiledXaml)
			{
				var layout = new AutoCommand(useCompiledXaml);

				var vm1 = new MockViewModel(1)
				{
					EnableButton1 = true,
				};

				var vm2 = new MockViewModel(2)
				{
					EnableButton1 = true,
				};

				layout.BindingContext = vm1;

				(layout.Button1 as IButtonController)
					?.SendClicked();

				Assert.True("VM1 Call DoButton1 with null" == vm1.CallsStack.LastOrDefault());

				layout.BindingContext = vm2;

				(layout.Button1 as IButtonController)
					?.SendClicked();

				Assert.True("VM2 Call DoButton1 with null" == vm2.CallsStack.LastOrDefault());

				Assert.False("VM2 Call DoButton1 with null" == vm1.CallsStack.LastOrDefault());
			}

			static IEnumerable Command_Execution_Test_Data()
			{
				return Concatenate(
					Command_Execution_Test_Data(useCompiledXaml:false),
					Command_Execution_Test_Data(useCompiledXaml:true)
					);					
			}

			static IEnumerable Command_Execution_Test_Data(bool useCompiledXaml)
			{
				yield return new object[]
				{
					useCompiledXaml,	// useCompiledXaml 
					false,				// enableButton1
					false,				// enableButton1
					new string[]	    // expectedCallsStack
					{
						"VM0 Call CanDoButton1 with null",		// It's calling from Command BindigProperty ctor
						"VM0 Call CanDoButton2 with Button 2",	// It's calling from Command BindigProperty ctor	
						"VM0 Call CanDoAll with null",			// It's calling from Command BindigProperty ctor
					}
				};
				yield return new object[]
				{
					useCompiledXaml,	// useCompiledXaml 
					true,				// enableButton1
					false,				// enableButton1
					new string[]	    // expectedCallsStack
					{
						"VM0 Call CanDoButton1 with null",		// It's calling from Command BindigProperty ctor
						"VM0 Call CanDoButton2 with Button 2",	// It's calling from Command BindigProperty ctor	
						"VM0 Call CanDoAll with null",			// It's calling from Command BindigProperty ctor
						"VM0 Set EnableButton1 with True",
						"VM0 Call CanDoButton1 with null",
						"VM0 Call CanDoAll with null",
						"VM0 Call DoButton1 with null",
					}
				};
				yield return new object[]
				{
					useCompiledXaml,	// useCompiledXaml 
					false,				// enableButton1
					true,				// enableButton1
					new string[]	    // expectedCallsStack
					{
						"VM0 Call CanDoButton1 with null",		// It's calling from Command BindigProperty ctor
						"VM0 Call CanDoButton2 with Button 2",	// It's calling from Command BindigProperty ctor	
						"VM0 Call CanDoAll with null",			// It's calling from Command BindigProperty ctor
						"VM0 Set EnableButton2 with True",
						"VM0 Call CanDoButton2 with Button 2",
						"VM0 Call CanDoAll with null",
						"VM0 Call DoButton2 with Button 2",
					}
				};
				yield return new object[]
				{
					useCompiledXaml,	// useCompiledXaml 
					true,				// enableButton1
					true,				// enableButton1
					new string[]	    // expectedCallsStack
					{
						"VM0 Call CanDoButton1 with null",		// It's calling from Command BindigProperty ctor
						"VM0 Call CanDoButton2 with Button 2",	// It's calling from Command BindigProperty ctor	
						"VM0 Call CanDoAll with null",			// It's calling from Command BindigProperty ctor
						"VM0 Set EnableButton1 with True",
						"VM0 Call CanDoButton1 with null",
						"VM0 Call CanDoAll with null",
						"VM0 Set EnableButton2 with True",
						"VM0 Call CanDoButton2 with Button 2",
						"VM0 Call CanDoAll with null",
						"VM0 Call DoButton1 with null",
						"VM0 Call DoButton2 with Button 2",
						"VM0 Call DoAll with null",
					}
				};
			}
		}

		class MockViewModel : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			int _Id;
			IList<string> _callsStack =
				new List<string>();
			public MockViewModel(int id)
			{
				_Id = id;
			}

			public IEnumerable<string> CallsStack
			{
				get { return _callsStack.ToArray(); }
			}

			public void ClearMessages()
			{
				_callsStack.Clear();
			}

			bool _enableButton1;
			public bool EnableButton1
			{
				get { return _enableButton1; }
				set
				{
					if (_enableButton1 == value)
						return;
					_enableButton1 = value;
					_callsStack
						.Add($"VM{_Id} Set {nameof(EnableButton1)} with {value}");
					OnPropertyChanged();
				}
			}

			bool _enableButton2;
			public bool EnableButton2
			{
				get { return _enableButton2; }
				set
				{
					if (_enableButton2 == value)
						return;
					_enableButton2 = value;
					_callsStack
						.Add($"VM{_Id} Set {nameof(EnableButton2)} with {value}");
					OnPropertyChanged();
				}
			}

			[DependedOn(nameof(EnableButton1))]
			bool CanDoButton1(object parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(CanDoButton1)} with {parameter ?? "null"}");
				return EnableButton1;
			}

			void DoButton1(object parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(DoButton1)} with {parameter ?? "null"}");

			}

			[DependedOn(nameof(EnableButton1))]
			bool CanDoButton1(string parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(CanDoButton1)} with string {parameter ?? "null"}");
				return EnableButton1;
			}

			void DoButton1(string parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(DoButton1)} with string {parameter ?? "null"}");
			}


			[DependedOn(nameof(EnableButton2))]
			bool CanDoButton2(object parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(CanDoButton2)} with {parameter ?? "null"}");
				return EnableButton2;
			}

			void DoButton2(object parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(DoButton2)} with {parameter ?? "null"}");

			}

			[DependedOn(nameof(EnableButton1))]
			[DependedOn(nameof(EnableButton2))]
			bool CanDoAll(object parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(CanDoAll)} with {parameter ?? "null"}");

				return EnableButton1 && EnableButton2;
			}

			void DoAll(object parameter)
			{
				_callsStack
					.Add($"VM{_Id} Call {nameof(DoAll)} with {parameter ?? "null"}");
			}


			protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		static IEnumerable Concatenate(params IEnumerable[] enumerables)
		{
			if (enumerables == null)
			{
				throw new ArgumentNullException(nameof(enumerables));
			}

			foreach (var enumerator in enumerables.Select(e=>e.GetEnumerator()))
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				} 
			}	
		}

	}
}