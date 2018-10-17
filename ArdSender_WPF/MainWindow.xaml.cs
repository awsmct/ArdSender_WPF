using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArdSender_WPF
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			int delay = 1000;
			GetIP serv = new GetIP();
			Task.Run(async () =>
			{
				HttpServer serv1 = new HttpServer(serv.ip.ToString());
				while (true)
			{
				UpdateVisitor updateVisitor = new UpdateVisitor();
				Computer computer = new Computer();
				computer.Open();
				computer.CPUEnabled = true;
				computer.GPUEnabled = true;
				computer.Accept(updateVisitor);

				for (int i = 0; i < computer.Hardware.Length; i++)
				{
					if (computer.Hardware[i].HardwareType == HardwareType.CPU) //Get CPU temp 
					{
						for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
						{
							if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
							{
								string str = ("CPU Temp: " + computer.Hardware[i].Sensors[j].Value.ToString() + "°C");
								await App.Current.Dispatcher.BeginInvoke(new Action(() => cpu.Text = str));
							}
						}
					}
				}
				for (int i = 0; i < computer.Hardware.Length; i++) //Get NVidia GPU temp
				{
					if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia)
					{
						for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
						{
							if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
							{
								string str = ("GPU Temp: " + computer.Hardware[i].Sensors[j].Value.ToString() + "°C");
									await App.Current.Dispatcher.BeginInvoke(new Action(() => { gpu.Text = str; ip.Text = serv.ip.ToString(); }));
							}
						}
					}
				}
					for (int i = 0; i < computer.Hardware.Length; i++) //Get CPU load
					{
						if (computer.Hardware[i].HardwareType == HardwareType.CPU)
						{
							for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
							{
								if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
								{
									string str = ("CPU Load: " + computer.Hardware[i].Sensors[j].Value.ToString() + "%");
									await App.Current.Dispatcher.BeginInvoke(new Action(() => { cpuLoad.Text = str; }));
								}
							}
						}
					}
					/*for (int i = 0; i < computer.Hardware.Length; i++) //Get NVidia GPU load (not working correctly)
					{
						if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia)
						{
							for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
							{
								if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Load)
								{
									string str = ("GPU Load: " + computer.Hardware[i].Sensors[j].Value.ToString() + "%");
									await App.Current.Dispatcher.BeginInvoke(new Action(() => { gpuLoad.Text = str; }));
								}
							}
						}
					}
					*/
					computer.Close();
					await Task.Delay(delay);
					await App.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
						serv1.Update(cpu.Text.ToString(), gpu.Text.ToString(), cpuLoad.Text.ToString(), gpuLoad.Text.ToString());
				}));
				}
			});
			
		}
		public class UpdateVisitor : IVisitor
		{
			public void VisitComputer(IComputer computer)
			{
				computer.Traverse(this);
			}
			public void VisitHardware(IHardware hardware)
			{
				hardware.Update();
				foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
			}
			public void VisitSensor(ISensor sensor) { }
			public void VisitParameter(IParameter parameter) { }
		}

	}
}
