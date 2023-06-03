using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MugenMvvmToolkit;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TestInterviewProject.Infrastructure;
using TestInterviewProject.Models;
using YLocalization;
using UserControl = System.Windows.Controls.UserControl;

namespace TestInterviewProject.Controls
{
    /// <summary>
    /// Interaction logic for WorkPlane.xaml
    /// </summary>
    public partial class WorkPlane : UserControl
    {
        protected Matrix4 LookAt = Matrix4.LookAt(0, 0, 0.50f, 0, 0, 0, 0, 2, 0);
        private Vector2d[] joints;
        private Vector2d[] jointsUnderMousePointer;
        private Vector2d[] selectedJoint;
        private Vector2d[] jointsCarret;
        private Vector2d[] liner;

        private int selectedIndex = -1;

        private readonly ICoordinateHelper coordinateHelper;

        private List<Chain> currentChainState = new List<Chain>();

        private readonly Mutex updateMutex = new Mutex();

        public static readonly DependencyProperty ChainsProperty = DependencyProperty.Register(
            "Chains", typeof(IEnumerable<Chain>), typeof(WorkPlane), new PropertyMetadata(default(IEnumerable<Chain>), OnChainsSet));

        private static void OnChainsSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WorkPlane workPlane)
            {
                workPlane.UpdateChains();
            }
        }

        public static readonly DependencyProperty DesiredJointPositionProperty = DependencyProperty.Register(
            "DesiredJointPosition", typeof(Joint), typeof(WorkPlane), new PropertyMetadata(default(Joint)));

        public Joint DesiredJointPosition
        {
            get => (Joint) GetValue(DesiredJointPositionProperty);
            set => SetValue(DesiredJointPositionProperty, value);
        }

        private void UpdateChains()
        {
            updateMutex.WaitOne();

            currentChainState = Chains.ToList();

            UpdateJoins();

            updateMutex.ReleaseMutex();

            RenderCurrentScene();
        }

        private void UpdateJoins()
        {
            joints = coordinateHelper.GetVertexFromChains(currentChainState);

            jointsCarret = new[]
            {
                new Vector2d(joints.Last().X - 0.02, 0.12),
                new Vector2d(joints.Last().X, 0.15),
                new Vector2d(joints.Last().X + 0.02, 0.12),
                new Vector2d(joints.Last().X + 0.02, 0.08),
                new Vector2d(joints.Last().X - 0.02, 0.08),
                new Vector2d(joints.Last().X - 0.02, 0.12),
            };
        }

        public IEnumerable<Chain> Chains
        {
            get => (IEnumerable<Chain>) GetValue(ChainsProperty);
            set => SetValue(ChainsProperty, value);
        }

        public WorkPlane()
        {
            InitializeComponent();

            coordinateHelper = ServiceProvider.IocContainer.Get<ICoordinateHelper>();

            liner = new[]
            {
                new Vector2d(0.0, 0.1),
                new Vector2d(1, 0.1),
            };
            jointsUnderMousePointer = new Vector2d[0];
            BindOrUnbind(true);
        }

        private void BindOrUnbind(bool bind)
        {
            if (bind)
            {
                Unloaded += OnUnloded;
                GlControl.MouseUp += OnMouseUp;
                GlControl.MouseDown += OnMouseDown;
                GlControl.MouseMove += OnMouseMove;
                GlControl.MouseLeave += OnMouseLeave;
            }
            else
            {
                Unloaded -= OnUnloded;
                GlControl.MouseUp -= OnMouseUp;
                GlControl.MouseDown -= OnMouseDown;
                GlControl.MouseMove -= OnMouseMove;
                GlControl.MouseLeave -= OnMouseLeave;
                
                GlControl.ContextMenu = null;
                GlControl.Dispose();

            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            selectedIndex = -1;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            selectedIndex = -1;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            var oldValue = selectedIndex;
            if (jointsUnderMousePointer != null && jointsUnderMousePointer.Any())
            {
                var selectedPoint = jointsUnderMousePointer[0];
                for (var index = 0; index < joints.Length; index++)
                {
                    if (Math.Abs(joints[index].X - selectedPoint.X) < double.Epsilon &&
                        Math.Abs(joints[index].Y - selectedPoint.Y) < double.Epsilon)
                    {
                        selectedIndex = index;
                        break;
                    }
                }
            }

            if (selectedIndex != oldValue)
            {
                RenderCurrentScene();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (joints == null || !joints.Any())
            {
                jointsUnderMousePointer = new Vector2d[0];
                return;
            }

            
            var relativeMouseX = 2.0 / GlControl.ClientSize.Width;
            var relativeMouseY = -2.0 / GlControl.ClientSize.Height;

            var newxMouseCoord = e.X;
            var newyMouseCoord = e.Y;

            var currentYCoord = (GlControl.ClientSize.Height - newyMouseCoord) * relativeMouseY / 2 + 1.0;
            currentYCoord = 1.0 - currentYCoord;
            var currentXCoord = -1.0 * newxMouseCoord * relativeMouseX / 2 + 1.0;
            currentXCoord = 1.0 - currentXCoord;

            if (selectedIndex != -1)
            {
                jointsUnderMousePointer = new Vector2d[0];

                DesiredJointPosition = new Joint
                {
                    X = currentXCoord,
                    Y = currentYCoord,
                    Name = selectedIndex.ToString()
                };

                return;
            }

            var deltaY = - 10.0 * relativeMouseY / 2;
            var deltaX = 10.0 * relativeMouseX / 2;


            jointsUnderMousePointer = joints.Where(j =>
                    Math.Abs(j.X - currentXCoord) <= Math.Abs(deltaX) &&
                    Math.Abs(j.Y - currentYCoord) <= Math.Abs(deltaY))
                .ToArray();
            RenderCurrentScene();
        }

        private void OnUnloded(object sender, RoutedEventArgs e)
        {
            BindOrUnbind(false);
            GlControl.Dispose();
            FormsHost.Dispose();
            GlControl = null;
            FormsHost = null;
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
                GL.ClearColor(Color.White);
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
            updateMutex.WaitOne();

            GlControl.MakeCurrent();
            GlControl.VSync = false;  //Сделано для совместимости со старыми картами NVidia

            GL.CallList(1);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.EnableClientState(ArrayCap.VertexArray);

            RenderLiner();

            RenderCarret();

            RenderBones();

            RenderJointUnderMouse();

            RenderJoints();

            RenderSelectedJoint();

            GL.DisableClientState(ArrayCap.VertexArray);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.PointSmooth);

            GlControl.SwapBuffers();

            updateMutex.ReleaseMutex();
        }

        private void RenderSelectedJoint()
        {
            if (joints == null || selectedIndex < 0 || selectedIndex > joints.Length - 1)
            {
                return;
            }
            selectedJoint = new []{joints[selectedIndex]};
            GL.Color3(Color.OrangeRed);
            GL.PointSize(17f);
            GL.VertexPointer(2, VertexPointerType.Double, 0, selectedJoint);
            GL.DrawArrays(BeginMode.Points, 0, selectedJoint.Length);
        }

        private void RenderCarret()
        {
            if (jointsCarret == null || jointsCarret.Length == 0)
            {
                return;
            }
            
            GL.Color3(Color.Black);
            GL.LineWidth(2.5f);

            GL.VertexPointer(2, VertexPointerType.Double, 0, jointsCarret);
            GL.DrawArrays(BeginMode.LineStrip, 0, jointsCarret.Length);
        }

        private void RenderLiner()
        {
            GL.Color3(Color.LightGray);
            GL.LineWidth(1.5f);

            GL.VertexPointer(2, VertexPointerType.Double, 0, liner);
            GL.DrawArrays(BeginMode.LineStrip, 0, liner.Length);
        }

        private void RenderBones()
        {
            if (joints == null || joints.Length == 0)
            {
                return;
            }
            GL.Color3(Color.Black);
            GL.LineWidth(2.5f);
            GL.VertexPointer(2, VertexPointerType.Double, 0, joints);
            GL.DrawArrays(BeginMode.LineStrip, 0, joints.Length - 1);
        }

        private void RenderJointUnderMouse()
        {
            if (jointsUnderMousePointer.Any())
            {
                GL.Color3(Color.DeepSkyBlue);
                GL.PointSize(15f);
                GL.VertexPointer(2, VertexPointerType.Double, 0, jointsUnderMousePointer);
                GL.DrawArrays(BeginMode.Points, 0, jointsUnderMousePointer.Length);
            }
        }

        private void RenderJoints()
        {
            if (joints == null || joints.Length == 0)
            {
                return;
            }
            GL.Color3(Color.Black);
            GL.PointSize(10f);
            GL.VertexPointer(2, VertexPointerType.Double, 0, joints);
            GL.DrawArrays(BeginMode.Points, 0, joints.Length);
            GL.Color3(Color.White);
            GL.PointSize(6);
            GL.VertexPointer(2, VertexPointerType.Double, 0, joints);
            GL.DrawArrays(BeginMode.Points, 0, joints.Length);
        }
    }
}
