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

        public static string PaintByNumbers = @"Turn a photo into a simple black-and-white coloring page by extracting clear outlines of all major shapes, people, objects, and important details. Remove background clutter, textures, colors, and small details, focusing only on bold, easily colorable areas suitable for children. Exclude any shading or grayscale; output only crisp and simple line art so the final image is clean, uncluttered, and ready for coloring.

**Detailed Steps:**
- Identify all main subjects in the photo (people, animals, prominent objects).
- Simplify each subject to its basic shapes and outlines.
- Omit unnecessary background details, shadows, or textures.
- Use thick, continuous lines for edges and important features; avoid thin or broken lines.
- No color or grayscale—only black outlines on a white background.
- Make sure the result is straightforward and clear, suitable for young children to color.

**Output Format:**  
Describe the coloring page in detail as a step-by-step written instruction suitable for an image-generation model, or—if the platform supports it—output a simplified SVG markup representing the coloring page.

**Example:**

*Input:* (Photo of a dog sitting in grass with trees behind.)

*Output (written description):*  
A simple coloring page of a sitting dog. The dog is outlined with bold lines, and its facial features—eyes, nose, mouth, and ears—are clearly marked. The grass beneath the dog is suggested by a few curved lines, and the background is left blank, omitting any trees or additional detail. There is no shading or color, just crisp black outlines on white.

*Output (SVG markup placeholder for a real image):*  
<svg>  
<!-- Outlines of dog body, face, and basic grass shapes as simple paths -->  
</svg>  

**Important:**  
- Focus on keeping all outlines bold, clear, and minimal.
- Ensure all shapes are closed and suited for easy coloring.
- Do not include any text, watermarks, or complex backgrounds.
- If something in the photo is unclear or too complex, omit it for clarity.

**REMINDER:**  
Your goal is to produce a simple, uncluttered line-art version of the photo, ready for children to color, with only essential shapes and outlines.";

        public static string sewingPrompt = @"Convert this garment into a detailed sewing pattern. Extract the structure of the clothing and redraw it as flat pattern pieces with clean black line art on a white background. Show front and back pieces, sleeves, collars, and any additional components. Include seam allowances, grainline arrows, notches, and fold lines. Present the output as a professional sewing pattern sheet, clearly labeled for cutting and assembly.";
    }

    
 }