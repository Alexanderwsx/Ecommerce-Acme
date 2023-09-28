document.addEventListener("DOMContentLoaded", function () {
    const imageInputs = document.querySelectorAll('.image-input');
    const previewContainer = document.getElementById('image-preview');

    imageInputs.forEach(inputField => {
        inputField.addEventListener('change', function () {
            // Limpia el contenedor de vista previa
            while (previewContainer.firstChild) {
                previewContainer.removeChild(previewContainer.firstChild);
            }

            // Agrega la vista previa para cada imagen seleccionada
            imageInputs.forEach(input => {
                const files = input.files;
                Array.from(files).forEach(file => {
                    const reader = new FileReader();

                    reader.onload = function (e) {
                        const img = document.createElement('img');
                        img.src = e.target.result;
                        img.style.maxWidth = '100px'; // Set the width of image preview
                        img.classList.add('mr-2', 'mb-2'); // Add some margin for spacing

                        const removeBtn = document.createElement('button');
                        removeBtn.textContent = 'Remove';
                        removeBtn.onclick = function () {
                            previewContainer.removeChild(img);
                            previewContainer.removeChild(removeBtn);
                            input.value = ''; // Clear the input
                        };

                        previewContainer.appendChild(img);
                        previewContainer.appendChild(removeBtn);
                    }

                    reader.readAsDataURL(file);
                });
            });
        });
    });
});
