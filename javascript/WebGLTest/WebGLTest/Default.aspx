<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebGLTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
 
    uniform sampler2D sampler2d;
 
    varying float v_Dot;
    varying vec2 v_texCoord;
 
    void main()
    {
        vec2 texCoord = vec2(v_texCoord.s, 1.0 - v_texCoord.t);
        vec4 color = texture2D(sampler2d, texCoord);
        color += vec4(0.1, 0.1, 0.1, 1);
        gl_FragColor = vec4(color.xyz * v_Dot, color.a);
    }
    </script>
    <script type="text/javascript">
        function createDynamicTexture(gl, width, height) {
            var pixels = new Int32Array(width * height);
            var texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
            //  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
            //  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
            gl.pixelStorei(gl.UNPACK_ALIGNMENT, 1);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, width, height, 0, gl.RGBA, gl.UNSIGNED_BYTE, pixels);

            return texture;
        }

        function createCheckerboardTexture(gl) {
            var pixels = new Array([255, 255, 255,
                                             0, 0, 0,
                                             0, 0, 0,
                                             255, 255, 255]);
            var texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
            //  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
            //  gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
            gl.pixelStorei(gl.UNPACK_ALIGNMENT, 1);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, 2, 2, 0, gl.RGBA, gl.UNSIGNED_BYTE, pixels);

//            gl.glTexImage2D(GLAdapter.GL_TEXTURE_2D, 0, GLAdapter.GL_RGBA, HOLODECK_TEXTURE_SIZE, HOLODECK_TEXTURE_SIZE, 0, GLAdapter.GL_RGBA,
//			    GLAdapter.GL_UNSIGNED_BYTE, holoDeckTexture);

            return texture;
        }

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
            gl.uniform1i(gl.getUniformLocation(gl.program, "sampler2d"), 0);

            // Enable texturing
            gl.enable(gl.TEXTURE_2D);

            // Create a box. On return 'gl' contains a 'box' property with
            // the BufferObjects containing the arrays for vertices,
            // normals, texture coords, and indices.
            gl.box = makeBox(gl);

            // Load an image to use. Returns a WebGLTexture object
            spiritTexture = createDynamicTexture(gl, 128, 128);//  createCheckerboardTexture(gl);//  loadImageTexture(gl, "resources/spirit.jpg");

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

            // Bind the texture to use
            gl.bindTexture(gl.TEXTURE_2D, spiritTexture);

            // Draw the cube
            gl.drawElements(gl.TRIANGLES, gl.box.numIndices, gl.UNSIGNED_BYTE, 0);

            // Show the framerate
            framerate.snapshot();

//            currentAngle += incAngle;
//            if (currentAngle > 360)
//                currentAngle -= 360;
        }

        function start() {
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
            framerate = new Framerate("framerate");
            var f = function () {
                window.requestAnimFrame(f, c);
                drawPicture(gl);
            };
            f();

        }

        $(document).ready(function () { start(); });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <canvas id="example">
    If you're seeing this your web browser doesn't support the &lt;canvas>&gt; element. Ouch!
</canvas>
    <div id="framerate">
    </div>
    </form>
</body>
</html>
