namespace My_Ai.Util

{
    public static class Prompts
    {
        public static string ImproveCv = @"You are an expert in evaluating CVs and providing feedback. 
Please review the following CV and provide a detailed analysis of its strengths and weaknesses.
Focus on the structure, content, and overall presentation.
Provide suggestions for improvement where necessary.
The CV is attached";
        public static string CustomiseCv = @"You are an expert in writing cv's, please provide a cv tailored to the job description provided.
The CV is attached and the job description is: {0}";
    public static string GenerateCoverLetter = @"You are an expert in writing cover letters. 
Please write a cover letter customised to the job description provided. The CV is attached and the job description is: {0}";
    }
}
