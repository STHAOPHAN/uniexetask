import React, { useState, useEffect } from 'react';
import { Grid } from '@material-ui/core';
import Controls from "../../components/controls/Controls";
import { useForm, Form } from '../../components/useForm';

const initialFValues = {
    workshopId: 0,
    name: '',
    description: '',
    startDate: new Date(),
    endDate: new Date(),
    location: '',
    regUrl: '',
    status: ''
}

export default function WorkShopForm(props) {
    const { addOrEdit, recordForEdit } = props;

    const validate = (fieldValues = values) => {
        let temp = { ...errors }
        if ('name' in fieldValues)
            temp.name = fieldValues.name ? "" : "This field is required."
        if ('description' in fieldValues)
            temp.description = fieldValues.description ? "" : "This field is required."
        if ('startDate' in fieldValues)
            temp.startDate = fieldValues.startDate ? "" : "This field is required."
        if ('endDate' in fieldValues)
            temp.endDate = fieldValues.endDate ? "" : "This field is required."
        if ('location' in fieldValues)
            temp.location = fieldValues.location ? "" : "This field is required."
        if ('regUrl' in fieldValues)
            temp.regUrl = fieldValues.regUrl ? "" : "This field is required."
        setErrors({
            ...temp
        });

        if (fieldValues === values)
            return Object.values(temp).every(x => x === "")
    }

    const {
        values,
        setValues,
        errors,
        setErrors,
        handleInputChange,
        resetForm
    } = useForm(initialFValues, true, validate);

    const handleSubmit = e => {
        e.preventDefault();
        if (validate()) {
            addOrEdit(values, resetForm);
        }
    }

    useEffect(() => {
        if (recordForEdit != null)
            setValues({
                ...recordForEdit
            })
    }, [recordForEdit]);

    return (
        <Form onSubmit={handleSubmit}>
            <Grid container>
                <Grid item xs={6}>
                    <Controls.Input
                        name="name"
                        label="Workshop Name"
                        value={values.name}
                        onChange={handleInputChange}
                        error={errors.name}
                    />
                    <Controls.Input
                        label="Description"
                        name="description"
                        value={values.description}
                        onChange={handleInputChange}
                        error={errors.description}
                    />
                    <Controls.DatePicker
                        name="startDate"
                        label="Start Date"
                        value={values.startDate}
                        onChange={handleInputChange}
                        error={errors.startDate}
                    />
                    <Controls.DatePicker
                        name="endDate"
                        label="End Date"
                        value={values.endDate}
                        onChange={handleInputChange}
                        error={errors.endDate}
                    />
                </Grid>
                <Grid item xs={6}>
                    <Controls.Input
                        name="location"
                        label="Location"
                        value={values.location}
                        onChange={handleInputChange}
                        error={errors.location}
                    />
                    <Controls.Input
                        name="regUrl"
                        label="Registration URL"
                        value={values.regUrl}
                        onChange={handleInputChange}
                        error={errors.regUrl}
                    />
                    {/* <Controls.Checkbox
                        name="status"
                        label="Active Workshop"
                        value={values.status}
                        onChange={handleInputChange}
                    /> */}
                    <div>
                        <Controls.Button
                            type="submit"
                            text="Submit" />
                        <Controls.Button
                            text="Reset"
                            color="default"
                            onClick={resetForm} />
                    </div>
                </Grid>
            </Grid>
        </Form>
    )
}
