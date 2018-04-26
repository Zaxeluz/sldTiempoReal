using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Microsoft.Win32;
using System.Windows.Threading;



namespace Grabacion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveIn waveIn;
        WaveFormat formato;
        WaveFileWriter writer;
        WaveOutEvent output;
        AudioFileReader reader;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
            formato = waveIn.WaveFormat;



            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;

            writer =
                new WaveFileWriter("sonido2.wav", formato);

            waveIn.StartRecording();
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            writer.Dispose();

        }

        void OnDataAvailable (object sender, WaveInEventArgs e)
        {

            byte[] buffer = e.Buffer;
            int bytesGrabados = e.BytesRecorded;


            double acumulador = 0;
            double numMuestras = 0;

            for (int i=0; i < bytesGrabados; i+=2)
            {
                // byte i =  0 1 1 0 0 1 1 1
                //byte i+1 = 0 0 0 0 0 0 0 0 0 1 1 0 0 1 1 1
                // or      = 0 1 1 0 0 1 1 1 0 1 1 0 0 1 1 1
                short muestra =
                        (short)Math.Abs((buffer[i + 1] << 8)|buffer[i]);
                //lblMuestra.Text = muestra.ToString();
                //sldVolumen.Value = (double)muestra;
                acumulador += muestra;
                numMuestras++;
            }
            double promedio = acumulador / numMuestras;
            sldVolumen.Value = promedio;
            //writer.Write(buffer, 0, bytesGrabados);
           
        }

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            waveIn.StopRecording();
        }

        

        

        private void button_Click(object sender, RoutedEventArgs e)
        {
            output = new WaveOutEvent();
            reader = new AudioFileReader("sonido2.wav");
            output.Init(reader);
            output.Play();

        }
    }
}
