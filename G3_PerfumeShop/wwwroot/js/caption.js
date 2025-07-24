function addCaptionFields(input) {
    const captionsContainer = document.getElementById('captionsContainer');
    captionsContainer.innerHTML = ''; // Clear existing caption fields
    Array.from(input.files).forEach((file, index) => {
        const div = document.createElement('div');
        div.innerHTML = `
                    <label for="caption-${index}">Chú thích cho ${file.name}:</label>
                    <input type="text" name="captions[${index}]" id="caption-${index}" class="form-control" placeholder="Nhập chú thích...">
                `;
        captionsContainer.appendChild(div);
    });
}