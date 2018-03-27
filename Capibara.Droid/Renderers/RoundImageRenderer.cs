using System;
using System.ComponentModel;
using System.Reactive.Disposables;

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;

using Capibara.Droid.Renderers;
using Capibara.Forms;

using Reactive.Bindings.Extensions;
using Reactive.Bindings;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundImage), typeof(RoundImageRenderer))]
namespace Capibara.Droid.Renderers
{
    [Preserve(AllMembers = true)]
    public class RoundImageRenderer : ImageRenderer
    {
        protected CompositeDisposable Disposable { get; } = new CompositeDisposable();

        private Bitmap Bitmap
        {
            get
            {
                var drawable = this.Control.Drawable;
                if (drawable == null) return null;

                if (drawable is BitmapDrawable)
                {
                    return (drawable as BitmapDrawable).Bitmap;
                }

                Bitmap bitmap = null;
                try
                {
                    if (drawable is ColorDrawable)
                    {
                        bitmap = Bitmap.CreateBitmap(2, 2, Bitmap.Config.Argb8888);
                    }
                    else
                    {
                        bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
                    }

                    using(var bitmapCanvas = new Canvas(bitmap))
                    {
                        drawable.SetBounds(0, 0, bitmapCanvas.Width, bitmapCanvas.Height);
                        drawable.Draw(bitmapCanvas);   
                    }
                }
                catch
                {
                    bitmap = null;
                }

                return bitmap;
            }
        }

        public RoundImageRenderer(Context context) : base(context) { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Image.Source))
            {
                this.Disposable.Dispose();
                this.Disposable.Clear();
                this.bitmapPaint?.Dispose();
                this.bitmapPaint = null;
            }

            base.OnElementPropertyChanged(sender, e);
        }

        public override void Draw(Canvas canvas)
        {
            var borderWidth = (Element as RoundImage).BorderWidth;

            var strokeWidth = borderWidth > 0
                ? (float)Math.Ceiling(borderWidth * this.Context.Resources.DisplayMetrics.Density + .5f)
                : 0f;

            var drawableRect = new RectF();
            drawableRect.Set(this.CalculateBounds());
            drawableRect.Inset(strokeWidth - 1.0f, strokeWidth - 1.0f);

            var radius = (float)Math.Min(drawableRect.Width(), drawableRect.Height()) / 2f;

            var drawablePaint = this.BitmapPaint;
            if (drawablePaint != null)
                canvas.DrawCircle(drawableRect.CenterX(), drawableRect.CenterY(), radius - strokeWidth, drawablePaint);

            using (var paint = new Paint { AntiAlias = true, StrokeWidth = strokeWidth })
            using (var path = new Path())
            {
                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = (Element as RoundImage).BorderColor.ToAndroid();
                canvas.DrawCircle(drawableRect.CenterX(), drawableRect.CenterY(), radius - (strokeWidth / 2), paint);
            }
        }

        private Paint bitmapPaint;

        private Paint BitmapPaint
        {
            get
            {
                var bitmap = this.Bitmap;
                if (bitmap == null) return null;

                var borderWidth = (Element as RoundImage).BorderWidth;

                var strokeWidth = borderWidth > 0
                    ? (float)Math.Ceiling(borderWidth * this.Context.Resources.DisplayMetrics.Density + .5f)
                    : 0f;

                var drawableRect = new RectF();
                drawableRect.Set(this.CalculateBounds());
                drawableRect.Inset(strokeWidth - 1.0f, strokeWidth - 1.0f);

                var bitmapHeight = bitmap.Height;
                var bitmapWidth = bitmap.Width;

                float scale;
                float dx = 0;
                float dy = 0;

                var shaderMatrix = new Matrix();
                shaderMatrix.Set(null);

                if (bitmapWidth * drawableRect.Height() > drawableRect.Width() * bitmapHeight)
                {
                    scale = drawableRect.Height() / (float)bitmapHeight;
                    dx = (drawableRect.Width() - bitmapWidth * scale) * 0.5f;
                }
                else
                {
                    scale = drawableRect.Width() / (float)bitmapWidth;
                    dy = (drawableRect.Height() - bitmapHeight * scale) * 0.5f;
                }

                shaderMatrix.SetScale(scale, scale);
                shaderMatrix.PostTranslate((int)(dx + 0.5f) + drawableRect.Left, (int)(dy + 0.5f) + drawableRect.Top);

                var bitmapShader = new BitmapShader(bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                bitmapShader.SetLocalMatrix(shaderMatrix);

                var result = new Paint { AntiAlias = true };
                result.SetShader(bitmapShader);

                return this.bitmapPaint = result;
            }
        }

        private RectF CalculateBounds()
        {
            int availableWidth = Width - PaddingLeft - PaddingRight;
            int availableHeight = Height - PaddingTop - PaddingBottom;

            int sideLength = Math.Min(availableWidth, availableHeight);

            float left = PaddingLeft + (availableWidth - sideLength) / 2f;
            float top = PaddingTop + (availableHeight - sideLength) / 2f;

            return new RectF(left, top, left + sideLength, top + sideLength);
        }
    }
}
