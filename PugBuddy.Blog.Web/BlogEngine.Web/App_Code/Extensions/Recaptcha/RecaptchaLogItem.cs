// Copyright (c) 2007 Adrian Godong, Ben Maurer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Adapted for dotnetblogengine by Filip Stanek ( http://www.bloodforge.com )

using System;

namespace Recaptcha
{
    /// <summary>
    /// Summary description for RecaptchaLogItem
    /// </summary>
    [Serializable]
    public class RecaptchaLogItem
    {
        public string Response = String.Empty;
        public string Challenge = String.Empty;
        public Guid CommentID = Guid.Empty;
        public double TimeToComment = 0; // in seconds - this is the time from the initial page load until a captcha was successfully solved
        public double TimeToSolveCapcha = 0; // in seconds - this is the time from the last time the captcha was refreshed until it was successfully solved.
        public UInt16 NumberOfAttempts = 0;
        public bool Enabled = true;
        public bool Necessary = true;
    }
}