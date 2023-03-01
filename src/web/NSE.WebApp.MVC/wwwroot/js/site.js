function BuscaCep() {
    $(document).ready(function () {

        function limpa_formulário_cep() {
            // Cleans the values in the CEP form
            $("#Endereco_Logradouro").val("");
            $("#Endereco_Bairro").val("");
            $("#Endereco_Cidade").val("");
            $("#Endereco_Estado").val("");
        }

        // When the CEP looses the focus
        $("#Endereco_Cep").blur(function () {

            // New variable "cep" only with digits
            var cep = $(this).val().replace(/\D/g, '');

            // Checks if the field "cep" has a value
            if (cep != "") {

                // Regular expression to validate the CEP
                var validacep = /^[0-9]{8}$/;

                // Checks the CEP format
                if (validacep.test(cep)) {

                    // Fills fields with "..." while querying webservice
                    $("#Endereco_Logradouro").val("...");
                    $("#Endereco_Bairro").val("...");
                    $("#Endereco_Cidade").val("...");
                    $("#Endereco_Estado").val("...");

                    // Consult the webservice viacep.com.br/
                    $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?",
                        function (dados) {

                            if (!("erro" in dados)) {
                                // Updates the fields with the values from the consult to the webservice
                                $("#Endereco_Logradouro").val(dados.logradouro);
                                $("#Endereco_Bairro").val(dados.bairro);
                                $("#Endereco_Cidade").val(dados.localidade);
                                $("#Endereco_Estado").val(dados.uf);
                            }
                            else {
                                // Message in case the CEP searched was not found
                                limpa_formulário_cep();
                                alert("CEP não encontrado.");
                            }
                        });
                }
                else {
                    // Message in case the CEP is invalid
                    limpa_formulário_cep();
                    alert("Formato de CEP inválido.");
                }
            }
            else {
                // Cleans the form
                limpa_formulário_cep();
            }
        });
    });
}