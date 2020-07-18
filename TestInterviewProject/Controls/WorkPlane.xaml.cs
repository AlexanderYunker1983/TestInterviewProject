using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TestInterviewProject.Controls
{
    /// <summary>
    /// Interaction logic for WorkPlane.xaml
    /// </summary>
    public partial class WorkPlane : UserControl
    {
        protected Matrix4 LookAt = Matrix4.LookAt(0, 0, 0.50f, 0, 0, 0, 0, 2, 0);

        public WorkPlane()
        {
            InitializeComponent();

        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Task.Run(async () =>
            {
                await Task.Delay(100);
                Dispatcher.Invoke(() =>
                {
                    PrepareCallLists();
                    RenderCurrentScene();
                });
            });
        }

        private void PrepareCallLists()
        {
            GL.NewList(1, ListMode.Compile);
            {
                GL.Viewport(0, 0, GlControl.ClientSize.Width, GlControl.ClientSize.Height);
                GL.ClearColor(Color.LightGray);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref LookAt);
                GL.Translate(- 1.0, - 1.0, 0);
                GL.Scale(2.0, 2.0, 0);

                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.Enable(EnableCap.Blend);

                GL.Enable(EnableCap.LineSmooth);
                GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

                GL.Enable(EnableCap.PointSmooth);
                GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            }
            GL.EndList();
        }

        private void RenderCurrentScene()
        {
            GlControl.MakeCurrent();
            GlControl.VSync = false;  //Сделано для совместимости со старыми картами NVidia

            GL.CallList(1);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.PointSmooth);

            GlControl.SwapBuffers();
        }
    }
}
