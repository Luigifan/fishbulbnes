<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="nes.aspx.cs" Inherits="WebGLTest.nes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/Scripts/mscorlib.js"></script>
    <script src="Scripts/PPU.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Scripts/CpuArr.js"></script>
    <script type="text/javascript" src="/Scripts/CpuOps.js"></script>
    <script type="text/javascript" src="/Scripts/ScriptFX.Core.js"></script>
    <script type="text/javascript" src="/Scripts/bulbascript.debug.js"></script>
    <script type="text/javascript" src="/Scripts/jquery-1.4.1.js"></script>
    <script src="Scripts/dkrom.js" type="text/javascript"></script>
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="Scripts/webgl-utils.js" type="text/javascript"></script>
    <script src="Scripts/J3DIMath.js" type="text/javascript"></script>
    <script src="Scripts/J3DI.js" type="text/javascript"></script>
    <script id="vshader" type="x-shader/x-vertex"> 
    uniform mat4 u_modelViewProjMatrix;
    uniform mat4 u_normalMatrix;
    uniform vec3 lightDir;
 
    attribute vec3 vNormal;
    attribute vec4 vTexCoord;
    attribute vec4 vPosition;
 
    varying float v_Dot;
    varying vec2 v_texCoord;


    void main()
    {
        gl_Position = u_modelViewProjMatrix * vPosition;
        v_texCoord = vTexCoord.st;
        vec4 transNormal = u_normalMatrix * vec4(vNormal, 1);
        v_Dot = max(dot(transNormal.xyz, lightDir), 0.0);
    }
    </script>
    <script id="fshader" type="x-shader/x-fragment"> 
#ifdef GL_ES
    precision mediump float;
#endif
 
    uniform sampler2D myTexture;
    uniform sampler2D myPalette;
    
    varying float v_Dot;
    varying vec2 v_texCoord;
 
    float rand(vec2 n)
    {
      return 0.5 + 0.5 *
         fract(sin(dot(n.xy, vec2(12.9898, 78.233)))* 43758.5453);
    }

    void main()
    {
        vec2 texCoord = vec2(v_texCoord.s, 1.0 - v_texCoord.t);
        vec4 color = texture2D(myTexture, texCoord);
//        
//        vec4 palIndex = texture2D(myTexture, vec2(255,color.r)); 
//        
//        vec2 palCoord = vec2(palIndex.r, 0.0);
//vec4 finalColor = texture2D(myPalette,vec2(color.r, 0.5));

//        float coord = rand(v_texCoord);
//        vec4 finalColor = texture2D(myPalette,vec2(coord, coord));
//        
        gl_FragColor = vec4(color.rgb, 1.0);
        //gl_FragColor = vec4(color.r * 10.0, 0.0,0.0, 1.0);
   }
    </script>
    <script type="text/javascript">
        var glpixels = new Uint8Array(256 * 256 * 4);

        function createPaletteTexture(gl, length)
        {
            gl.activeTexture(1);
            var  paltex = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, paltex);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
            gl.pixelStorei(gl.UNPACK_ALIGNMENT, 1);

            gl.texImage2D(gl.TEXTURE_1D, 0, gl.RGBA, 256 , 1, 0, gl.RGBA, gl.UNSIGNED_BYTE, palette);
            gl.bindTexture(gl.TEXTURE_2D, null);
            return paltex;
        }

        function createDynamicTexture(gl, width, height) {
            //glpixels = new Int32Array(width * height);
            gl.activeTexture(0);
            var texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
            //  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
            //  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
            gl.pixelStorei(gl.UNPACK_ALIGNMENT, 1);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, width, height, 0, gl.RGBA, gl.UNSIGNED_BYTE, glpixels);
            gl.bindTexture(gl.TEXTURE_2D, null);
            return texture;
        }

        var paletteTexture;
        function init() {

            // Initialize
            var gl = initWebGL(
            // The id of the Canvas Element
            "example",
            // The ids of the vertex and fragment shaders
            "vshader", "fshader",
            // The vertex attribute names used by the shaders.
            // The order they appear here corresponds to their index
            // used later.
            ["vNormal", "vColor", "vPosition"],
            // The clear color and depth values
            [0, 0, 0.5, 1], 10000);
            if (!gl) {
                return;
            }
            // Set some uniform variables for the shaders
            gl.uniform3f(gl.getUniformLocation(gl.program, "lightDir"), 0, 0, 1);
            gl.uniform1i(gl.getUniformLocation(gl.program, "myTexture"), 0);
            gl.uniform1i(gl.getUniformLocation(gl.program, "myPalette"), 1);

            // Enable texturing
            gl.enable(gl.TEXTURE_2D);
            

            // Create a box. On return 'gl' contains a 'box' property with
            // the BufferObjects containing the arrays for vertices,
            // normals, texture coords, and indices.
            gl.box = makeBox(gl);

            // Load an image to use. Returns a WebGLTexture object
            spiritTexture = createDynamicTexture(gl, 256, 256); //  createCheckerboardTexture(gl);//  loadImageTexture(gl, "resources/spirit.jpg");
            
            gl.activeTexture(gl.TEXTURE0);
            gl.bindTexture(gl.TEXTURE_2D, spiritTexture);

            paletteTexture = createPaletteTexture(gl, 256);

            gl.activeTexture(gl.TEXTURE1);
            gl.bindTexture(gl.TEXTURE_2D, paletteTexture);


            // Create some matrices to use later and save their locations in the shaders
            gl.mvMatrix = new J3DIMatrix4();
            gl.u_normalMatrixLoc = gl.getUniformLocation(gl.program, "u_normalMatrix");
            gl.normalMatrix = new J3DIMatrix4();
            gl.u_modelViewProjMatrixLoc =
                gl.getUniformLocation(gl.program, "u_modelViewProjMatrix");
            gl.mvpMatrix = new J3DIMatrix4();

            // Enable all of the vertex attribute arrays.
            gl.enableVertexAttribArray(0);
            gl.enableVertexAttribArray(1);
            gl.enableVertexAttribArray(2);

            // Set up all the vertex attributes for vertices, normals and texCoords
            gl.bindBuffer(gl.ARRAY_BUFFER, gl.box.vertexObject);
            gl.vertexAttribPointer(2, 3, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, gl.box.normalObject);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, gl.box.texCoordObject);
            gl.vertexAttribPointer(1, 2, gl.FLOAT, false, 0, 0);

            // Bind the index array
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, gl.box.indexObject);

            return gl;
        }

        width = -1;
        height = -1;

        function reshape(gl) {
            var canvas = document.getElementById('example');
            if (canvas.width == width && canvas.height == height)
                return;

            width = canvas.width;
            height = canvas.height;

            // Set the viewport and projection matrix for the scene
            gl.viewport(0, 0, width, height);
            gl.perspectiveMatrix = new J3DIMatrix4();
            gl.perspectiveMatrix.perspective(30, width / height, 1, 10000);
            gl.perspectiveMatrix.lookat(0, 0, 7, 0, 0, 0, 0, 1, 0);
        }

        function drawPicture(gl) {
            // Make sure the canvas is sized correctly.
            reshape(gl);

            // Clear the canvas
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            // Make a model/view matrix.
            gl.mvMatrix.makeIdentity();
            gl.mvMatrix.rotate(20, 1, 0, 0);
            gl.mvMatrix.rotate(currentAngle, 0, 1, 0);

            // Construct the normal matrix from the model-view matrix and pass it in
            gl.normalMatrix.load(gl.mvMatrix);
            gl.normalMatrix.invert();
            gl.normalMatrix.transpose();
            gl.normalMatrix.setUniform(gl, gl.u_normalMatrixLoc, false);

            // Construct the model-view * projection matrix and pass it in
            gl.mvpMatrix.load(gl.perspectiveMatrix);
            gl.mvpMatrix.multiply(gl.mvMatrix);
            gl.mvpMatrix.setUniform(gl, gl.u_modelViewProjMatrixLoc, false);

//            gl.activeTexture(gl.TEXTURE0);
//            spiritTexture = createDynamicTexture(gl, 256, 256);
            gl.bindTexture(gl.TEXTURE_2D, spiritTexture);
            // Draw the cube
            gl.drawElements(gl.TRIANGLES, gl.box.numIndices, gl.UNSIGNED_BYTE, 0);

            
            gl.activeTexture(gl.TEXTURE1);
            gl.bindTexture(gl.TEXTURE_2D, paletteTexture);

            // Show the framerate
            //framerate.snapshot();

            currentAngle += incAngle;
            if (currentAngle > 360)
                currentAngle -= 360;
        }

        function startGl() {
            startNes();
            var c = document.getElementById("example");
            var w = Math.floor(window.innerWidth * 0.9);
            var h = Math.floor(window.innerHeight * 0.9);

            c.width = w;
            c.height = h;

            var gl = init();
            if (!gl) {
                return;
            }

            currentAngle = 0;
            incAngle = 0.5;
            //framerate = new Framerate("framerate");
            
            setGlContext(gl);
            var f = function () {
                window.requestAnimFrame(f, c);
                runFrame();
//                if (needFrame) {
//                    needFrame = false;
//                    setTimeout(runFrame(), 1);
//                }
                drawPicture(gl);
            };
            f();

        }

        
    </script>
    <script src="Scripts/tendo.js" type="text/javascript"></script>
</head>
<body>
    <asp:PlaceHolder ID="romDude" runat="server" />
    <label id="framespersec" />
    <canvas id="canvas1" width="256" height="240"></canvas>
    <canvas id="example">
    If you're seeing this your web browser doesn't support the &lt;canvas>&gt; element. Ouch!
</canvas>
    <input type="button" id="Button3" onclick="startGl();" value="start gl" />
</body>
</html>
