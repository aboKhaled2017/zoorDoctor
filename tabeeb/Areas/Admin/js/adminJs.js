
/*$(document).ready(function () {
    $('#PersonTableContainer').jtable({
        title: 'Table of People',
        actions: {
            listAction: '/Admin/index/PersonList',
            createAction: '/Admin/index/createPerson',
            updateAction: '/Admin/index/updatePerson',
            deleteAction: '/Admin/index/deletePerson'
        },
        fields: {
            id: {
                key: true,
                list: false
            },
            name: {
                title: 'Name',
                width: '20%'
            },
            mail: {
                title: 'Email',
                width: '20%',
                type: 'email'
            },
            price: {
                title: 'Price',
                width: '20%',
                type: 'number'
            },
            birthdate: {
                title: 'Birth Date',
                type: 'date',
                width: '20%',
                create: false,
                edit: false
            },
            education: {
                title: 'Education',
                width: '15%'
            },
            type: {
                title: 'Type',
                width: '15%',
                list: false,
                type: 'radiobutton',
                options: { 'false': 'male', 'true': 'female' },
                defaultValue: 'false'
            },

            docSpId: {
                title: 'Select doctor speciality',
                create: true,
                edit: true,
                list: false,
                dependsOn: ['id'],
                options: ['الباطنة','النسا والتوليد','امراض جلدية']

            }
        }
});
    $('#PersonTableContainer').jtable('load');
});*/