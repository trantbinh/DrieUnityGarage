HinhAnh.onchange = evt => {

    const [file] = HinhAnh.files

    if (file) {

        previewnen.src = URL.createObjectURL(file);

        $("#previewnen").removeClass("d-none");

    }

}

fileImageCT1.onchange = evt => {

    const [file] = fileImageCT1.files

    if (file) {

        preview1.src = URL.createObjectURL(file);

        $("#preview1").removeClass("d-none");

    }

}

fileImageCT2.onchange = evt => {

    const [file] = fileImageCT2.files

    if (file) {

        preview2.src = URL.createObjectURL(file);

        $("#preview2").removeClass("d-none");

    }

}

fileImageCT3.onchange = evt => {

    const [file] = fileImageCT3.files

    if (file) {

        preview3.src = URL.createObjectURL(file);

        $("#preview3").removeClass("d-none");

    }

}

fileImageCT4.onchange = evt => {

    const [file] = fileImageCT4.files

    if (file) {

        preview4.src = URL.createObjectURL(file);

        $("#preview4").removeClass("d-none");

    }

}