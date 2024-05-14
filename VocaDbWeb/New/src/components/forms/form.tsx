"use client";

import React from "react";
import { AcceptsOnChange, Form } from "./input";

export const SongForm = ({ children }: { children: AcceptsOnChange[] }) => {
	return <Form submit={(key) => console.log(key)}>{children}</Form>;
};

