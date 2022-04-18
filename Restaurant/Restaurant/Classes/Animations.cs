using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Restaurant.Classes
{
    class Animations
    {
        public static TranslateTransform borderAnimation(int anim, Border control, Boolean Property)
        {
            Storyboard storyboard = new Storyboard();
            control.Opacity = 0;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = anim;
            myDoubleAnimation.Duration = TimeSpan.FromMilliseconds(500);

            TranslateTransform translateTransform = new TranslateTransform();
            if (Property)
            {
                translateTransform.BeginAnimation(TranslateTransform.XProperty, myDoubleAnimation);
            }
            else
                translateTransform.BeginAnimation(TranslateTransform.YProperty, myDoubleAnimation);
            control.BeginAnimation(UIElement.OpacityProperty, myDoubleAnimation);
            return translateTransform;

        }
        /*
          public static TranslateTransform shakingControl(TextBlock control)
         {
             Storyboard storyboard = new Storyboard();
             DoubleAnimation myDoubleAnimation = new DoubleAnimation();
             myDoubleAnimation.From = 0;
             myDoubleAnimation.To = 10;
             myDoubleAnimation.Duration = TimeSpan.FromMilliseconds(50);
             control.BeginAnimation(UIElement.OpacityProperty, myDoubleAnimation);

             TranslateTransform translateTransform = new TranslateTransform();
             translateTransform.BeginAnimation(TranslateTransform.XProperty, myDoubleAnimation);
             control.BeginAnimation(UIElement.OpacityProperty, myDoubleAnimation);
             return translateTransform;

         }
         */


        //Create a Blink animation
    public static void CreateBlinkAnimation(TextBlock control)
    {
        var switchOffAnimation = new DoubleAnimation
        {
            To = 0,
            Duration = TimeSpan.Zero
        };

        var switchOnAnimation = new DoubleAnimation
        {
            To = 1,
            Duration = TimeSpan.Zero,
            BeginTime = TimeSpan.FromSeconds(0.5)
        };

        var blinkStoryboard = new Storyboard
        {
            Duration = TimeSpan.FromSeconds(1),
            RepeatBehavior = RepeatBehavior.Forever
        };

        Storyboard.SetTarget(switchOffAnimation, control);
        Storyboard.SetTargetProperty(switchOffAnimation, new PropertyPath(Canvas.OpacityProperty));
        blinkStoryboard.Children.Add(switchOffAnimation);

        Storyboard.SetTarget(switchOnAnimation, control);
        Storyboard.SetTargetProperty(switchOnAnimation, new PropertyPath(Canvas.OpacityProperty));
        blinkStoryboard.Children.Add(switchOnAnimation);

        control.BeginStoryboard(blinkStoryboard);

    }

        public static void shakingControl(TextBlock control)
        {

            TranslateTransform translateTransform = new TranslateTransform();

            var shake1 = new DoubleAnimation
            {
                To = 15,
                Duration = TimeSpan.Zero
            };

            var shake2 = new DoubleAnimation
            {
                To = -15,
                Duration = TimeSpan.FromSeconds(0.1),
                BeginTime = TimeSpan.FromSeconds(0.1)
            };


            var shake3 = new DoubleAnimation
            {
                To = 10,
                Duration = TimeSpan.FromSeconds(0.1),
                BeginTime = TimeSpan.FromSeconds(0.2)
            };
            var shake4 = new DoubleAnimation
            {
                To = -10,
                Duration = TimeSpan.FromSeconds(0.1),
                BeginTime = TimeSpan.FromSeconds(0.3)
            };
            var shake5 = new DoubleAnimation
            {
                To = 5,
                Duration = TimeSpan.FromSeconds(0.1),
                BeginTime = TimeSpan.FromSeconds(0.4)
            };
            var shake6 = new DoubleAnimation
            {
                To = -5,
                Duration = TimeSpan.FromSeconds(0.1),
                BeginTime = TimeSpan.FromSeconds(0.5)
            };

            var shake7 = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.1),
                BeginTime = TimeSpan.FromSeconds(0.6)
            };


            translateTransform.BeginAnimation(TranslateTransform.XProperty,shake1);
            translateTransform.BeginAnimation(TranslateTransform.XProperty,shake2);
            translateTransform.BeginAnimation(TranslateTransform.XProperty,shake3);
            translateTransform.BeginAnimation(TranslateTransform.XProperty,shake4);
            translateTransform.BeginAnimation(TranslateTransform.XProperty,shake5);
            translateTransform.BeginAnimation(TranslateTransform.XProperty,shake6);
            translateTransform.BeginAnimation(TranslateTransform.XProperty, shake7);
            control.RenderTransform = translateTransform;


        }
    }
}
